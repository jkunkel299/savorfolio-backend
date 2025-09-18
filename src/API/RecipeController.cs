using Microsoft.AspNetCore.Mvc;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Models.DTOs;

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

    public static void MapRecipeSearch(this WebApplication app)
    {
        // single API for recipe search by optional filters
        app.MapPost("/api/recipes/search", async (
            [FromBody] RecipeFilterRequestDTO filter,
            RecipeService recipeService) =>
        {
            var recipes = await recipeService.SearchRecipesAsync(filter);
            return Results.Ok(recipes);
        });
    }
}