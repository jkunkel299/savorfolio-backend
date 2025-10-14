using Microsoft.AspNetCore.Http;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace Tests.Helpers;

public static class RecipeEndpointsHelper
{
    // replicates the API endpoint defined in RecipeEndpoints.MapRecipeSearch
    public static async Task<IResult> InvokeRecipeSearchEndpoint(RecipeFilterRequestDTO filter, IRecipeService service)
    {
        var results = await service.SearchRecipesAsync(filter);
        return Results.Ok(results);
    }   

    // replicates the API endpoint defined in RecipeEndpoints.MapRecipeById
    public static async Task<IResult> InvokeRecipeViewEndpoint(int recipeId, IViewRecipeService service)
    {
        var recipe = await service.CompileRecipeAsync(recipeId);
        return Results.Ok(recipe);
    }   
}