using System.Text.RegularExpressions;
using AngleSharp.Dom;
using savorfolio_backend.Interfaces;
using FuzzySharp;
using System.Diagnostics.CodeAnalysis;

namespace savorfolio_backend.LogicLayer.WebScraper;

public partial class IngredientParseService(IUnitsRepository unitsRepository, IIngredientRepository ingredientRepository) : IIngredientParseService
{
    private readonly IUnitsRepository _unitsRepository = unitsRepository;
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;

    public /* async Task<List<string>> */ List<string> ExtractIngredients(IDocument document, string ingredientsPattern = "")
    {
        List<string> rawIngredients = [];
        // List<IngredientListDTO> ingredientsList = [];
        // List<string> ingredientsList = [];
        // call function to get ingredient elements by pattern
        if (ingredientsPattern != "")
        {
            rawIngredients = GetIngredientsByPattern(document, ingredientsPattern);
        }

        // or call function to get ingredient elements by fallback heuristic
        if (rawIngredients.Count == 0)
        {
            // fallback
            rawIngredients = IngredientFallback(document);
        }
        return rawIngredients;

        // for (int i = 0; i < rawIngredients.Count; i++)
        // {

        //     string ingredientLine = rawIngredients[i];
        //     ingredientLine = CheckboxRegex().Replace(ingredientLine, string.Empty);
        //     // extract quantity
        //     var (quantity, ingMinusQuant) = ExtractQuantity(ingredientLine);

        //     // extract unit
        //     var (unitName, /* unitId,  */ingAndQualifier) = await ExtractUnit(ingMinusQuant);

        //     // extract ingredient + qualifier
        //     var (ingredientName, /* ingredientId, */ qualifier) = await ExtractIngredientQualifier(ingAndQualifier);

        //     build DTO
        //     var ingredientDto = new IngredientListDTO
        //     {
        //         IngredientOrder = i,
        //         // IngredientId = ingredientId,
        //         IngredientName = ingredientName,
        //         Quantity = quantity,
        //         // UnitId = unitId,
        //         UnitName = unitName,
        //         Qualifier = qualifier
        //     };

        //     // add to ingredient list
        //     ingredientsList.Add(ingredientDto);
        //     ingredientsList.Add(string.Join(" | ", quantity, unitName, ingAndQualifier));
        // }

        // return ingredientsList;
    }

    #region GetIngredientsByPattern
    // get ingredient elements by pattern
    public List<string> GetIngredientsByPattern(IDocument document, string ingredientsPattern)
    {
        List<string> ingredients = [];
        string query = $".{ingredientsPattern}";
        // if the pattern is mv-create-ingredients
        if (ingredientsPattern == "mv-create-ingredients")
        {
            // adjust the query to get only list items in the labeled div
            query = $"div.{ingredientsPattern} li";
        }
        var ingredientsElements = document.QuerySelectorAll(query);

        List<string> useIngs = [];
        // if the ingredientsElements is just one long string
        if (ingredientsElements.Length == 1)
        {
            // split by \n character and add elements to the list useIngs
            useIngs = [.. ingredientsElements[0].TextContent.Split('\n')];
        }
        else
        {
            // otherwise, add each element's text content to useIngs
            foreach (var item in ingredientsElements)
            {
                useIngs.Add(item.TextContent.Trim());
            }
        }

        foreach (var addIng in useIngs)
        {
            // clean each item in the list to remove checkboxes
            var cleaned = CheckboxRegex.Replace(addIng, string.Empty);
            // remove unicode space characters
            cleaned = SpaceRegex.Replace(cleaned, " ");
            // remove excessive whitespace
            cleaned = WhitespaceRegex.Replace(cleaned, string.Empty);
            // if the string is empty, skip it
            if (string.IsNullOrEmpty(cleaned)) continue;
            // otherwise add the string to the final ingredient string list
            ingredients.Add(cleaned);
        }
        return ingredients;
    }
    #endregion



    // Get ingredient elements by fallback heuristic
    #region Fallback
    public List<string> IngredientFallback(IDocument document)
    {
        string term = "Ingredients";
        List<string> extractIngredients = [];
        // List<string> draftIngredients = []; // for use in general matching without class names
        var contentRoot = document.QuerySelector("[class*='entry-content']") ?? document.Body;

        // look for items with "ingredient" in the class name or id
        var tryIngredientsElements = contentRoot?
            .QuerySelectorAll(
                @"[class*='ingredient'] li, [class*='ingredient'] div, 
                [id*='ingredient'] li, [id*='ingredient'] div"
            );
        if (tryIngredientsElements != null)
        {
            foreach (var item in tryIngredientsElements)
            {
                string addIng = item.TextContent.Trim();
                var cleaned = CheckboxRegex.Replace(addIng, string.Empty);
                cleaned = SpaceRegex.Replace(cleaned, " ");
                cleaned = Regex.Replace(cleaned, @"\*", string.Empty);
                cleaned = Regex.Replace(cleaned, @"^\s*\d+\s*$(\r?\n)?", string.Empty, RegexOptions.Multiline);
                if (cleaned == string.Empty) continue;
                if (cleaned.Length < 400) extractIngredients.Add(cleaned.Trim());
            }
        }

        if (extractIngredients.Count == 0 || extractIngredients.Count > 35)
        {
            extractIngredients = [];
            // if no matches, look for the element with "Ingredients" as its text content
            // then look for lists and div after that element

            // Match the content the established term
            var ingredientLabel = document.All
                .FirstOrDefault(e => e.TextContent.Trim()
                .Equals(term, StringComparison.OrdinalIgnoreCase));
            // if there's a match for the label
            if (ingredientLabel != null)
            {
                // get the elements that follow the label
                var followingElements = ingredientLabel
                    .ParentElement?
                    .Children
                    .SkipWhile(c => c != ingredientLabel)
                    .Skip(1) // skip the label itself
                    .TakeWhile(c => c.Matches("li") || c.Matches("ul"));
                // join the elements into a string
                string ingString = string.Join(", ", followingElements!.Select(e => e.TextContent.Trim()));
                // trim whitespace 
                string ingStringWhitespace = WhitespaceRegex.Replace(ingString, "\n");
                // split into individual ingredients
                extractIngredients = [.. ingStringWhitespace.Split("\n")];
            }
            else
            {
                // last resort, look for any unordered list
                var ulElements = document.Body?.QuerySelectorAll("ul li");
                if (ulElements != null)
                {
                    extractIngredients.AddRange(
                        ulElements
                            .Select(li => li.TextContent.Trim())
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                    );
                }

            }
        }
        // if still no draft instructions, return empty list
        if (extractIngredients.Count == 0) return ["Could not find ingredients"];
        List<string> flagged = [];
        foreach (var ing in extractIngredients)
        {
            // (var unit, _) = await ExtractUnit(ing);
            // (var quantity, _) = ExtractQuantity(ing);
            // if (unit == "none") flagged.Add(ing);
            if (ing.Contains("Ingredients") || ing.Contains("ingredients")) flagged.Add(ing);
            if (ing.Contains('\t')) flagged.Add(ing);
        }
        foreach (var ing in flagged)
        {
            extractIngredients.Remove(ing);
        }
        return extractIngredients;
    }

    #endregion









    #region Split
    // TODO - split a single ingredient term into its tokens
    [ExcludeFromCodeCoverage]
    public List<string> SplitIngredientIntoTokens(string rawIngredient)
    {
        var splitIngredients = rawIngredient.Split(" ");
        return [.. splitIngredients];
    }
    #endregion



    #region Quantity
    // TODO - extract quantity
    [ExcludeFromCodeCoverage]
    public (string quantity, string ingMinusQuant) ExtractQuantity(string ingredient)
    {
        string quantity = "none";
        string ingMinusQuant;

        var quantityMatch = QuantityRegex.Match(ingredient);
        if (quantityMatch.Success)
        {
            quantity = quantityMatch.Value.Trim();
            ingMinusQuant = ingredient[quantityMatch.Length..].Trim();
        }
        else
        {
            ingMinusQuant = ingredient;
        }

        return (quantity, ingMinusQuant);
    }
    #endregion



    #region Unit
    // TODO - extract unit
    [ExcludeFromCodeCoverage]
    public async Task<(string unitName, /* int unitId, */ string remainder)> ExtractUnit(string rawIngredient)
    {
        string unit = "none";
        string tryUnit;
        string unitTerm = "";
        string ingAndQualifier = rawIngredient;
        try
        {
            // string unitName = "";
            // int unitId = 0;
            // var cleaned = Regex.Replace(rawIngredient, @"\(.*?\)", string.Empty);
            var cleaned = Regex.Replace(rawIngredient, @"\u00A0", " ");
            // split the ingredient at each space
            var splitIngredient = SplitIngredientIntoTokens(cleaned);
            foreach (var term in splitIngredient)
            {
                unitTerm = term;
                tryUnit = await _unitsRepository.UnitSearchReturnString(term);
                if (tryUnit != "none")
                {
                    unit = tryUnit;
                    break;
                }
                else
                {
                    unit = "none";
                }
                ;
            }

            Regex rawUnitRegex = new(unitTerm);
            // remove the unit from the ingredient line
            ingAndQualifier = rawUnitRegex.Replace(rawIngredient, string.Empty, 1).Trim();
        }
        catch (RegexParseException)
        {
            unit = "none";
        }
        // if no unit keep whole line
        if (unit == "none") ingAndQualifier = rawIngredient.Trim();
        return (unit, /* unitId,  */ingAndQualifier);
    }
    #endregion



    #region Ingredient/Qualifier
    // TODO - extract ingredient and qualifier
    [ExcludeFromCodeCoverage]
    public async Task<(string ingredientName, /* int ingredientId,  */string qualifier)> ExtractIngredientQualifier(string ingAndQualifier)
    {
        if (string.IsNullOrWhiteSpace(ingAndQualifier))
            return ("", "");

        ingAndQualifier = ingAndQualifier.Trim().ToLowerInvariant();

        var words = ingAndQualifier
            .Replace(",", "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        string bestIngredient = "";
        double bestScore = 0;

        // Generate substrings (longest to shortest)
        for (int start = 0; start < words.Count; start++)
        {
            for (int end = words.Count; end > start; end--)
            {
                var candidate = string.Join(' ', words.Skip(start).Take(end - start));

                var dtoList = await _ingredientRepository.IngredientSearchReturnString(candidate);

                if (dtoList == null || dtoList.Count == 0)
                    continue;

                var match = GetBestMatch(dtoList, candidate);

                // crude score = similarity by string length ratio
                double score = (double)match.Length / candidate.Length;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIngredient = match;
                }
            }
        }

        string qualifier = ingAndQualifier.Replace(bestIngredient, "").Trim();
        qualifier = qualifier.Trim(',', ' ');

        return (bestIngredient, qualifier);

        // string colorPattern = @"(red|brown|green|white)\b";
        // string ingredientName;
        // // int ingredientId;
        // List<string> qualifier = [];
        // string ingredientRaw;
        // string ingMinusQual = "";

        // List<string> splitAtComma = [.. ingAndQualifier.Split(",")];
        // if (splitAtComma.Count > 1)
        // {
        //     ingredientRaw = splitAtComma[0];
        //     splitAtComma.RemoveAt(0);
        //     string joined = string.Join("", splitAtComma);
        //     qualifier.Add(joined);
        // }
        // else
        // {
        //     ingredientRaw = ingAndQualifier;
        // }

        // List<string> splitAtSpace = [.. ingredientRaw.Split(" ")];
        // if (splitAtSpace.Count > 1)
        // {
        //     if (splitAtSpace[0].EndsWith("ed"))
        //     {
        //         qualifier.Add(splitAtSpace[0]);
        //     }
        //     else if (splitAtSpace[0].EndsWith("ly"))
        //     {
        //         if (splitAtSpace[1].EndsWith("ed"))
        //         {
        //             qualifier.Add(splitAtSpace[0]);
        //             qualifier.Add(splitAtSpace[1]);
        //         }
        //     }
        // }

        // var qualifierString = string.Join(" ", qualifier);
        // var qualifierToMatch = qualifierString.Split(" ");

        // foreach (string word in qualifierToMatch)
        // {
        //     if (ingMinusQual.Contains(word)) 
        //     ingMinusQual = ingredientRaw.Replace(word, string.Empty); // Replace the word with an empty string
        // }

        // var ingMatch = await _ingredientRepository.IngredientSearchReturnString(ingMinusQual);
        // // (ingredientName, ingredientId) = GetBestMatch(ingMatch, ingredientRaw);
        // ingredientName = GetBestMatch(ingMatch, ingredientRaw);

        // return (ingredientName, /* ingredientId, */ qualifierString);
    }
    #endregion



    // TODO - best match
    #region GetBestMatch
    // public static (string, int) GetBestMatch<TDTO>(List<TDTO> dtoList, string input) where TDTO : IDTOInterface
    [ExcludeFromCodeCoverage]
    public /* ( */string/* , int) */ GetBestMatch(List<string> dtoList, string input)
    {
        // initialize integer list for fuzz ratios
        List<int> fuzzRatio = [];
        // for each value in the returned list, score how well they match the given input
        foreach (var item in dtoList)
        {
            int wRatioScore = Fuzz.WeightedRatio(item/* .Name */, input);
            fuzzRatio.Add(wRatioScore);
        }
        // get the max ratio match
        int bestRatio = fuzzRatio.Max();
        // get the index of the best match
        var bestRatioIndex = fuzzRatio.IndexOf(bestRatio);
        // get the element from the list that best matches the label, using the max match
        var bestMatchName = dtoList[bestRatioIndex]/* .Name */;
        // var bestMatchId = dtoList[bestRatioIndex].Id;
        // return the best match
        return bestMatchName/* , bestMatchId */;
    }
    #endregion



    #region Regex
    // [GeneratedRegex(@"^\s*(\d+\s\d+/\d+|\d+/\d+|\d+(\.\d+)?|[¼½¾⅐⅑⅒⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞])", RegexOptions.IgnoreCase, "en-US")]
    // [ExcludeFromCodeCoverage]
    // private static partial Regex QuantityRegex();
    private static readonly Regex QuantityRegex = new(@"^\s*(\d+\s\d+/\d+|\d+/\d+|\d+(\.\d+)?|[¼½¾⅐⅑⅒⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    // [GeneratedRegex(@"▢")]
    // [ExcludeFromCodeCoverage]
    // private static partial Regex CheckboxRegex();
    private static readonly Regex CheckboxRegex = new(@"▢", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    // [GeneratedRegex(@"\u00A0")]
    // [ExcludeFromCodeCoverage]
    // private static partial Regex SpaceRegex();
    private static readonly Regex SpaceRegex = new(@"\u00A0", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    // [GeneratedRegex(@"\s{2,}")]
    // [ExcludeFromCodeCoverage]
    // private static partial Regex WhitespaceRegex();
    private static readonly Regex WhitespaceRegex = new(@"\s{2,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    #endregion
}