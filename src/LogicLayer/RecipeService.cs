using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class RecipeService(IRecipeRepository recipeRepository) : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;

    public async Task<List<RecipeDTO>> SearchRecipesAsync(RecipeFilterRequestDTO filter)
    {
        var recipes = await _recipeRepository.ReturnRecipesFiltered(filter);
        return recipes;
    }
}