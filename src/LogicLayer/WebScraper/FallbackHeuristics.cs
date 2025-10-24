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
    #region Description
    public static string ExtractDescription(IDocument document)
    {
        string recipeDescription = "";
        var tryDescriptionElement = document.Body?.QuerySelector("[class*='recipe-description']");
        var trySummary = document.Body?.QuerySelector("[class*='summary']");
        var tryGeneralDescription = document.Body?.QuerySelector("[class*='description']");

        if (tryDescriptionElement != null)
        {
            recipeDescription = tryDescriptionElement.TextContent.Trim();
        }
        else if (trySummary != null)
        {
            recipeDescription = trySummary?.TextContent.Trim() ?? "";
        }
        else if (tryGeneralDescription != null)
        {
            recipeDescription = tryGeneralDescription?.TextContent.Trim() ?? "";
        }

        return recipeDescription;
    }
    #endregion


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


    #region Prep/Cook Time
    public static string ExtractTimeNearLabel(IDocument document, string labelPattern)
    {
        var timeRegex = TimeRegex();

        // find all elements that contain the label pattern
        var labelElements = document.All
            .Where(e => e.TextContent != null &&
                        e.TextContent.Contains(labelPattern, StringComparison.CurrentCultureIgnoreCase) &&
                        e.TextContent.Any(char.IsDigit));

        string bestMatch = GetBestMatch(labelElements, labelPattern);
        // trim using regex to only include the time and unit
        var matchTrim = timeRegex.Match(bestMatch);
        if (matchTrim.Success)
        {
            return matchTrim.Value.Trim();
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
            return "";
        }        

        return "default";
    }
    #endregion

    // Regex generation
    [GeneratedRegex(@"\b\d+\s*(min|mins|minute|minutes|hr|hrs|hour|hours|sec|secs|second|seconds)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TimeRegex();
    [GeneratedRegex(@"\b\d+\s*(servings: |yield: | servings)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ServingsRegex();
}
