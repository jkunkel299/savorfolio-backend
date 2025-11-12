using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IIngListRepository
{
    int AddNewRecipeIng(
        List<IngredientListDTO> ingredientsData,
        int recipeId,
        List<SectionDTO>? sectionsData
    );
    Task<List<IngredientListDTO>> GetIngredientsByRecipeAsync(int recipeId);
}
