using AngleSharp;
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
    public static string GetBestMatch(IEnumerable<IElement> labelElements, string labelPattern/* , List<string>? labelPatterns */)
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


    
    #region Title
    public static string ExtractTitle(IDocument document)
    {
        string recipeTitle = "";
        // var tryTitle = document.Body?.QuerySelector("h2, h3, h4, [class*='recipe'], [class*='title']") ?? null;
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
        if (recipeDescription == "" && tryTitle != null)
        {
            for (int i = 0; i < 10; i++)
            {
                if (tryTitle.NextElementSibling!.TextContent.Length > 30)
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
        var timeRegex = TimeRegex();
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
            var matchTrim = timeRegex.Match(bestMatch);
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

        // string regexPattern = @"[^\p{N}\p{P}\p{S}]"; 
        string regexPattern = "[^0-9-]";

        var labelElements = document.All
                    .Where(e => e.TextContent != null &&
                        e.TextContent.Contains(patternMatch, StringComparison.CurrentCultureIgnoreCase) &&
                        e.TextContent.Any(char.IsDigit));

        if (patternMatch != "none" && labelElements.Any())
        {
            string bestMatch = GetBestMatch(labelElements, patternMatch);
            // trim using regex to only include the time and unit
            var matchTrim = Regex.Replace(bestMatch, regexPattern, string.Empty).Replace(":", "").Trim();
            if (matchTrim != null || matchTrim != "")
            {
                if (matchTrim!.Length < 4) return matchTrim!;
            }
        }        

        return "";
    }
    #endregion

    // Regex generation
    [GeneratedRegex(@"\b\d+\s*(min|mins|minute|minutes|hr|hrs|hour|hours|sec|secs|second|seconds)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TimeRegex();
    [GeneratedRegex(@"\b\d+\s*(servings: |yield: | servings)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ServingsRegex();
    [GeneratedRegex(@"recipe.*title", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TitleRegex();
}
