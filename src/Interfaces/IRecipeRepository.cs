using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IRecipeRepository
{
    Task<RecipeDTO> ReturnRecipeByIdAsync(int recipeId);
    Task<List<RecipeDTO>> ReturnRecipesFiltered(RecipeFilterRequestDTO filter);
    Task<int> AddNewRecipeAsync(RecipeDTO recipeData);
}