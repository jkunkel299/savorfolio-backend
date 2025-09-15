using savorfolio_backend.LogicLayer;

namespace savorfolio_backend.API;

public static class RecipeEndpoints
{
    public static void MapRecipeApi(this WebApplication app)
    {
        // get all recipes
        app.MapGet("/api/recipes", async (
            RecipeService recipeService) =>
        {
            var results = await recipeService.ReturnRecipesAsync();
            return Results.Ok(results);
        });
    }

    public static void MapRecipeIncludeIngredient(this WebApplication app)
    {
        // get recipes by ingredient ID
        app.MapGet("/api/recipes/includeIngredient", async (
            int ingredientId,
            RecipeService recipeService) =>
        {
            var results = await recipeService.ReturnRecipeByIngredientAsync(ingredientId);
            return Results.Ok(results);
        });
    }
}