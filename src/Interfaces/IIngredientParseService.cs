using AngleSharp.Dom;

namespace savorfolio_backend.Interfaces;

public interface IIngredientParseService
{
    List<string> ExtractIngredients(IDocument document, string ingredientsPattern = "");
    List<string> GetIngredientsByPattern(IDocument document, string ingredientsPattern);
    List<string> IngredientFallback(IDocument document);
    List<string> SplitIngredientIntoTokens(string rawIngredient);
    (string quantity, string ingMinusQuant) ExtractQuantity(string ingredient);
    Task<(string unitName, string remainder)> ExtractUnit(string rawIngredient);
    Task<(string ingredientName, string qualifier)> ExtractIngredientQualifier(string ingAndQualifier);
    string GetBestMatch(List<string> dtoList, string input);
}