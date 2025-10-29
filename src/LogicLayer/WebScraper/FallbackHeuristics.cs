using AngleSharp.Dom;
using FuzzySharp;
using FuzzySharp.Extractor;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Utils;
using savorfolio_backend.Models.enums;
using System.Text.RegularExpressions;
using AngleSharp.Common;

namespace savorfolio_backend.LogicLayer.WebScraper;

public partial class FallbackHeuristics
{
    #region GetBestMatch
    public static string GetBestMatch(IEnumerable<IElement> labelElements, string labelPattern)
    {
        // initialize integer list for fuzz ratios
        List<int> fuzzRatio = [];
        // for each element that contains the pattern, score how well they match the expected label
        foreach (var label in labelElements)
        {
            int wRatioScore = Fuzz.WeightedRatio(labelPattern, label.TextContent);
            fuzzRatio.Add(wRatioScore);
        }
        // get the max ratio match
        int bestRatio = fuzzRatio.Max();
        // get the index of the best match
        var bestRatioIndex = fuzzRatio.IndexOf(bestRatio);
        // get the element from the list that best matches the label, using the max match
        var bestMatch = labelElements.GetItemByIndex(bestRatioIndex).TextContent.ToLower();
        // return the best match
        return bestMatch;
    }
    #endregion


    #region MatchEnum
    public static string MatchEnum<TEnum>(IDocument document) where TEnum : Enum
    {
        string documentText = document.Body?.TextContent ?? string.Empty;
        string returnValue = "";
        var enumList = EnumExtensions.GetEnumList<TEnum>();
        var patternMatch = enumList.FirstOrDefault(t => documentText.Contains(t, StringComparison.OrdinalIgnoreCase)) ?? "none";
        var labelElements = document.All
                    .Where(e => e.TextContent != null &&
                        e.TextContent.Contains(patternMatch, StringComparison.OrdinalIgnoreCase));
        if (patternMatch != "none" && labelElements.Any())
        {
            string bestMatch = GetBestMatch(labelElements, patternMatch);
            ExtractedResult<string> bestTypeMatch = Process.ExtractOne(bestMatch, enumList);
            if (bestTypeMatch != null) returnValue = bestTypeMatch.Value;
        }
        return returnValue;
    }
    #endregion


    #region Title
    public static string ExtractTitle(IDocument document)
    {
        string recipeTitle = "";

        var tryTitle = document.All
            .FirstOrDefault(e => e.ClassList.Any(c => TitleRegex().IsMatch(c)));
        if (tryTitle != null)
        {
            recipeTitle = tryTitle?.TextContent.Trim() ?? "";
        }
        else
        {
            var metaTitle = document.QuerySelector("meta[property='og:title']")?.GetAttribute("content");
            if (!string.IsNullOrEmpty(metaTitle)) recipeTitle = metaTitle;
        }
        return recipeTitle;
    }
    #endregion


    #region Description
    public static string ExtractDescription(IDocument document)
    {
        string recipeDescription = "";
        var tryTitle = document.All
            .FirstOrDefault(e => e.ClassList.Any(c => TitleRegex().IsMatch(c)));
        var tryDescriptionElement = document.Body?.QuerySelector("[class*='recipe-description']");
        var trySummary = document.Body?.QuerySelector("[class*='summary']");

        // first, try to find an element that has a class name including "recipe-description"
        if (recipeDescription == "" && tryDescriptionElement != null)
        {
            recipeDescription = tryDescriptionElement.TextContent.Trim();
        }
        // if no match, try to find an element that has a class name including "summary"
        if (recipeDescription == "" && trySummary != null)
        {
            recipeDescription = trySummary?.TextContent.Trim() ?? "";
        }
        // if still no match, try to find elements close to the recipe title long enough to be a description
        // added does not contain "!" as the summaries usually contain this character
        if (recipeDescription == "" || !recipeDescription.Contains('!') && tryTitle != null) 
        {
            for (int i = 0; i < 10; i++)
            {
                if (tryTitle?.NextElementSibling!.TextContent.Length > 30)
                {
                    recipeDescription = tryTitle.NextElementSibling!.TextContent;
                    break;
                }
                i++;
            }
        }

        return recipeDescription;
    }
    #endregion

    #region Prep/Cook Time
    public static string ExtractTimeNearLabel(IDocument document, string labelPattern)
    {
        string bestMatch;

        // find all elements that contain the label pattern
        var labelElements = document.All
            .Where(e => e.TextContent != null &&
                        e.TextContent.Contains(labelPattern, StringComparison.CurrentCultureIgnoreCase) &&
                        e.TextContent.Any(char.IsDigit));
        if (labelElements.Any())
        {
            bestMatch = GetBestMatch(labelElements, labelPattern);
            // trim using regex to only include the time and unit
            var matchTrim = TimeRegex().Match(bestMatch);
            if (matchTrim.Success)
            {
                return matchTrim.Value.Trim();
            }
        } 
        return "";
    }
    #endregion

    #region Servings
    public static string ExtractServings(IDocument document)
    {
        // set the options for return
        string[] terms = ["servings", "yield", "yields", "serves"];

        // Access document text
        string documentText = document.Body?.TextContent ?? string.Empty;

        // Match the classNames to established patterns
        var patternMatch = terms.FirstOrDefault(t => documentText.Contains(t, StringComparison.OrdinalIgnoreCase)) ?? "none";

        string regexPattern = "[^0-9-]";

        var labelElements = document.All
                    .Where(e => e.TextContent != null &&
                        e.TextContent.Contains(patternMatch, StringComparison.OrdinalIgnoreCase) &&
                        e.TextContent.Any(char.IsDigit));

        if (patternMatch != "none" && labelElements.Any())
        {
            string bestMatch = GetBestMatch(labelElements, patternMatch);
            // trim using regex to only include the servings number
            var matchTrim = Regex.Replace(bestMatch, regexPattern, string.Empty).Replace(":", "").Trim();
            if (matchTrim != null || matchTrim != "")
            {
                if (matchTrim!.Length < 4) return matchTrim!;
            }
        }

        return "";
    }
    #endregion


    #region Tags
    public static TagStringsDTO ExtractTags(IDocument document)
    {
        string documentText = document.Body?.TextContent ?? string.Empty;
        if (documentText == string.Empty) return new TagStringsDTO();

        // recipe type/course
        string recipe_type = MatchEnum<RecipeTypeTag>(document);

        // cuisine
        string cuisine = MatchEnum<CuisineTag>(document);

        // meal type
        string meal = MatchEnum<MealTag>(document);

        // dietary
        List<string> dietary = ExtractDietaryTags(documentText);

        var recipeTags = new TagStringsDTO
        {
            Recipe_type = recipe_type,
            Cuisine = cuisine,
            Meal = meal,
            Dietary = dietary
        };

        return recipeTags;
    }
    #endregion
    

    #region Dietary
    public static List<string> ExtractDietaryTags(string documentText)
    {
        List<string> dietary = [];
        var dietaryList = EnumExtensions.GetEnumList<DietaryTag>();
        var dietaryDraft = dietaryList
            .Where(tag => documentText.Contains(tag, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        foreach (var item in dietaryDraft)
        {
            ExtractedResult<string> bestTypeMatch = Process.ExtractOne(item, dietaryList);
            if (bestTypeMatch != null) dietary.Add(bestTypeMatch.Value);
        };
        return dietary;
    }
    #endregion
    

    #region Instructions
    public static List<InstructionDTO> ExtractInstructions(IDocument document)
    {
        string[] terms = ["Instructions", "Directions"];
        List<string> draftInstructions = [];
        List<InstructionDTO> instructionDTOs = [];
        // look for items with "instruction" in the class name
        var tryInstructionsElements = document.Body?.QuerySelectorAll("[class*='instruction'] li, [class*='instruction'] ol li, [class*='instruction'] ul li, [class*='instruction'] p");
        if (tryInstructionsElements != null)
        {
            draftInstructions.AddRange(
               tryInstructionsElements
                   .Select(li => li.TextContent.Trim())
                   .Where(text => !string.IsNullOrWhiteSpace(text))
           );
        } 
        
        if (draftInstructions.Count == 0)
        {
            // if no matches, look for the element with "Instructions" or "Directions" as its text content
            // then look for ordered lists after that element
            // then look for unordered lists after that element

            // Match the content to one of the established terms
            var instructionLabel = document.All.FirstOrDefault(e =>
                terms.Any(t => e.TextContent.Trim().Equals(t, StringComparison.OrdinalIgnoreCase)));
            // if there's a match for the label
            if (instructionLabel != null)
            {
                // get the elements that follow the label
                var followingElements = instructionLabel
                    .ParentElement?
                    .Children
                    .SkipWhile(c => c != instructionLabel)
                    .Skip(1) // skip the label itself
                    .TakeWhile(c => c.Matches("div, ol, ul, li"));
                // join the elements into a string
                string insString = string.Join(", ", followingElements!.Select(e => e.TextContent.Trim()));
                // trim whitespace 
                string insStringWhitespace = WhitespaceRegex().Replace(insString, "\n");
                // remove "Step #" labels
                string insStringTrim = StepRegex().Replace(insStringWhitespace, "\n").Trim();
                // split into individual steps
                draftInstructions = [.. insStringTrim.Split("\n")];

            }
            else
            { // last resort, look for any ordered list
                var olElements = document.Body?.QuerySelectorAll("ol li");
                if (olElements != null)
                {
                    draftInstructions.AddRange(
                        olElements
                            .Select(li => li.TextContent.Trim())
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                    );
                }
            }
        }
        // if still no draft instructions, return empty list
        if (draftInstructions.Count == 0) return [];
        // build the instruction DTO list
        for (int i = 0; i < draftInstructions.Count; i++)
        {
            var instructionItem = new InstructionDTO
            {
                StepNumber = i+1,
                InstructionText = draftInstructions[i]
            };
            instructionDTOs.Add(instructionItem);
        }
        
        return instructionDTOs;
    }
    #endregion

    #region Regex
    // Regex generation
    [GeneratedRegex(@"\b\d+\s*(min|mins|minute|minutes|hr|hrs|hour|hours|sec|secs|second|seconds)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TimeRegex();

    [GeneratedRegex(@"recipe.*title", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TitleRegex();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"Step\s+\d+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex StepRegex();
    #endregion
}