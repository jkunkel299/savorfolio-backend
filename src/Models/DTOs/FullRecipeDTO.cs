namespace savorfolio_backend.Models.DTOs;

public class FullRecipeDTO
{
    public required int RecipeId { get; set; }

    public required RecipeDTO RecipeSummary { get; set; }

    public required TagStringsDTO RecipeTags { get; set; }

    public required List<IngredientListDTO> Ingredients { get; set; }

    public required List<InstructionDTO> Instructions { get; set; }
}