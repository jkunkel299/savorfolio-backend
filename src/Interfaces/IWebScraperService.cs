using AngleSharp.Dom;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IWebScraperService
{
    Task<DraftRecipeDTO> RunScraper(string url);
    Task<IDocument> GetHtmlAsync(string url);
    string SampleCssClasses(IDocument document);
    Dictionary<string, string?>? MapCssClassPatterns(string pattern);
    RecipeDTO BuildRecipeSummary(
        IDocument document,
        string titlePattern = "",
        string descriptionPattern = "",
        string prepTimePattern = "",
        string cookTimePattern = "",
        string servingsPattern = ""
    );
    List<string> BuildRecipeIngredients(IDocument document, string ingredientsPattern = "");
    List<InstructionDTO> BuildRecipeInstructions(IDocument document, string instructionsPattern = "");
    TagStringsDTO BuildRecipeTags(IDocument document, string coursePattern = "", string cuisinePattern = "");
}