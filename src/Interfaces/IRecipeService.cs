using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeDTO>> SearchRecipesAsync(RecipeFilterRequestDTO filter);
}