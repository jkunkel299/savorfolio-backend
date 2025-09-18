using savorfolio_backend.DataAccess;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class RecipeService(RecipeRepository recipeRepository)
{
    private readonly RecipeRepository _recipeRepository = recipeRepository;

    public async Task<List<Recipe>> ReturnRecipesAsync()
    {
        // Call the repository method (Data Access Layer)
        var recipes = await _recipeRepository.ReturnAllRecipes();

        return recipes;
    }

    public async Task<List<RecipeDTO>> SearchRecipesAsync(RecipeFilterRequestDTO filter)
    {
        var recipes = await _recipeRepository.ReturnRecipesFiltered(filter);
        return recipes;
    }
}