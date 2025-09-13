using savorfolio_backend.DataAccess;
using System.Text.Json;

namespace savorfolio_backend.LogicLayer;

public class RecipeService(RecipeRepository recipeRepository)
{
    private readonly RecipeRepository _recipeRepository = recipeRepository;

    public async Task<string> ReturnRecipesAsync()
    {
        // Call the repository method (Data Access Layer)
        var recipes = await _recipeRepository.ReturnAllRecipes();

        // Convert to JSON
        string recipesJson = JsonSerializer.Serialize(recipes);
        
        return recipesJson;
    }
}