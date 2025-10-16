using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    
    // replicates the API endpoint defined in AddRecipeEndpoints.MapManualRecipe
    public static async Task<IResult> InvokeAddManualRecipeEndpoint(string jsonBody, IAddRecipeService service)
    {
        // // Build a mock HttpRequest with the JSON body
        // var context = new DefaultHttpContext();
        // var request = context.Request;
        // var bodyBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        // request.Body = new MemoryStream(bodyBytes);

        // // extract delegate logic from the endpoint
        // using var reader = new StreamReader(request.Body);
        // var requestBody = await reader.ReadToEndAsync();

        // JObject newRecipe;
        // try
        // {
        //     newRecipe = JObject.Parse(requestBody);
        // }
        // catch (JsonReaderException ex)
        // {
        //     return Results.BadRequest($"Invalid JSON format: {ex.Message}");
        // }

        JsonDocument newRecipeBody;
        try
        {
            newRecipeBody = JsonDocument.Parse(jsonBody);
        }
        catch (System.Text.Json.JsonException ex)
        {
            return Results.BadRequest($"Invalid JSON format: {ex.Message}");
        }

        // Simulate what your production endpoint does
        var newRecipe = JObject.Parse(newRecipeBody.RootElement.GetRawText());
        
        OperationResult<int> result = await service.AddRecipeManually(newRecipe);
        if (result.Success)
        {
            return Results.Ok($"Recipe ID {result.Data} added successfully");
        }
        else
        {
            return Results.Problem("Recipe not added successfully");
        }
    }
}