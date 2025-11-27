using AngleSharp.Common;
using AngleSharp.Dom;
using FuzzySharp;
using FuzzySharp.Extractor;
using savorfolio_backend.Interfaces;

namespace savorfolio_backend.Utils;

public class HeuristicExtensions() : IHeuristicExtensions
{
    public string GetBestMatch(IEnumerable<IElement> labelElements, string labelPattern)
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

    public virtual string MatchEnum<TEnum>(IDocument document)
        where TEnum : Enum
    {
        var documentContent = document.Body?.QuerySelector("[class*='content']");

        // var documentText = documentContent?.QuerySelector("[class*='entry-footer']")?.TextContent ?? "";
        // if (documentText == "" )
        // {
        //     documentText = documentContent?.QuerySelector("[class*='post-terms']")?.TextContent ?? document.Body?.TextContent;
        // }

        string returnValue = "";
        var enumList = EnumExtensions.GetEnumList<TEnum>();
        var patternMatch =
            enumList.FirstOrDefault(
                t => documentContent!.TextContent.Contains(t, StringComparison.OrdinalIgnoreCase),
                "none"
            ) ?? "none";
        var labelElements = document.All.Where(e =>
            e.TextContent != null
            && e.TextContent.Contains(patternMatch, StringComparison.OrdinalIgnoreCase)
        );
        if (patternMatch != "none" && labelElements.Any())
        {
            string bestMatch = GetBestMatch(labelElements, patternMatch);
            ExtractedResult<string> bestTypeMatch = Process.ExtractOne(bestMatch, enumList);
            if (bestTypeMatch != null)
                returnValue = bestTypeMatch.Value;
        }
        return returnValue;
    }
}
