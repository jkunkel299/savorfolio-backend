namespace savorfolio_backend.Models.DTOs;

public class DraftRecipeDTO
{
    public required RecipeDTO RecipeSummary { get; set; }

    public required TagStringsDTO RecipeTags { get; set; }

    public required List<string> IngredientsString { get; set; }

    public required List<InstructionDTO> Instructions { get; set; }
}