using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IRecipeRepository
{
    Task<List<RecipeDTO>> ReturnRecipesFiltered(RecipeFilterRequestDTO filter);
    Task<int> AddNewRecipe(RecipeDTO recipeData);
}