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
    #region Fallback for description
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

    public static string GetBestMatch(IEnumerable<IElement> labelElements, string? labelPattern/* , List<string>? labelPatterns */)
    {
        if (labelPattern != null)
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

        /* if (labelPatterns?.Count > 0)
        {
            int rows = labelPatterns.Count;
            int cols = labelElements.Count();
            int[,] fuzzRatio2D = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)// foreach (var label in labelElements)
                {
                    int wRatioScore = Fuzz.WeightedRatio(labelPatterns[i], labelElements.ElementAt(j).TextContent);
                    fuzzRatio2D[i, j] = wRatioScore;//.Add(wRatioScore);
                }
            }
            // int maxRatio = fuzzRatio2D.Max(innerList => innerList.Count != 0 ? innerList.Max() : int.MinValue);
            // int maxVal = int.MinValue;
            // int bestRatioIndex = -1;

            // // Assuming all inner lists have the same number of columns
            // int numColumns = fuzzRatio2D.First().Count;

            // for (int col = 0; col < numColumns; col++)
            // {
            //     foreach (List<int> row in fuzzRatio2D)
            //     {
            //         if (row[col] > maxVal)
            //         {
            //             maxVal = row[col];
            //             bestRatioIndex = col;
            //         }
            //     }
            // }
            var maxInfo = Enumerable.Range(0, rows)
                .SelectMany(r => Enumerable.Range(0, cols)
                    .Select(c => new { Value = fuzzRatio2D[r, c], Row = r, Col = c }))
                .Aggregate((currentMax, next) => (next.Value > currentMax.Value) ? next : currentMax);
            var bestRatioIndex = maxInfo.Col;
        
            // get the element from the list that best matches the label, using the max match
            var bestMatch = labelElements.GetItemByIndex(bestRatioIndex).TextContent.ToLower();
            // return the best match
            return bestMatch;
        } */
        return "";
    }


    #region Fallback for Time
    public static string ExtractTimeNearLabel(IDocument document, string labelPattern)
    {
        var timeRegex = TimeRegex();

        // find all elements that contain the label pattern
        var labelElements = document.All
            .Where(e => e.TextContent != null &&
                        e.TextContent.Contains(labelPattern, StringComparison.CurrentCultureIgnoreCase) &&
                        e.TextContent.Any(char.IsDigit));

        string bestMatch = GetBestMatch(labelElements: labelElements, labelPattern: labelPattern/* , labelPatterns: null */);
        // trim using regex to only include the time and unit
        var matchTrim = timeRegex.Match(bestMatch);
        if (matchTrim.Success)
        {
            return matchTrim.Value.Trim();
        }
        return "";
    }
    #endregion

    #region Fallback Servings
    public static string ExtractServings(IDocument document)
    {
        var servingsRegex = ServingsRegex();
        // var searchTerms = new List<string> { "yield", "servings", "serves" };
        var labelElements = document.All
            .Where(e => e.TextContent != null && servingsRegex.IsMatch(e.TextContent.ToLower()));
        string bestMatch = GetBestMatch(labelElements: labelElements, labelPattern: "serving"/* , labelPatterns: searchTerms */);
        var matchTrim = Regex.Replace(bestMatch, servingsRegex.ToString(), string.Empty).Trim();
        return matchTrim;
    }
    #endregion

    // Regex generation
    [GeneratedRegex(@"\b\d+\s*(min|mins|minute|minutes|hr|hrs|hour|hours|sec|secs|second|seconds)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex TimeRegex();
    [GeneratedRegex(@"\b\d+\s*(servings: |yield: | servings)\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ServingsRegex();
}
