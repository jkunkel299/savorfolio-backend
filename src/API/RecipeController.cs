/* API Layer for recipe-related functionality */

using Microsoft.AspNetCore.Mvc;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.API;

public static class RecipeEndpoints
{
    public static void MapRecipeSearch(this WebApplication app)
    {
        // single API for recipe search by optional filters
        app.MapPost("/api/recipes/search", async (
            [FromBody] RecipeFilterRequestDTO filter,
            IRecipeService recipeService) =>
        {
            var recipes = await recipeService.SearchRecipesAsync(filter);
            return Results.Ok(recipes);
        });
    }

    public static void MapRecipeById(this WebApplication app)
    {
        app.MapGet("/api/recipes/view", async (
            int recipeId,
            IViewRecipeService viewRecipeService) =>
        {
            var recipe = await viewRecipeService.CompileRecipeAsync(recipeId);
            return Results.Ok(recipe);
        });
    }
}