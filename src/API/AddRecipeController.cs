/* API layer to add recipe manually, may also include adding recipe via scraping */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.API;

public static class AddRecipeEndpoints
{
    public static void MapManualRecipe(this WebApplication app)
    {
        app.MapPost("/api/recipes/add/manual", async (
            HttpRequest request,
            IAddRecipeService addRecipeService) =>
        {
            using var reader = new StreamReader(request.Body);
            var requestBody = await reader.ReadToEndAsync();

            JObject newRecipe;
            try
            {
                newRecipe = JObject.Parse(requestBody);
            }
            catch (JsonReaderException ex)
            {
                return Results.BadRequest($"Invalid JSON format: {ex.Message}");
            }

            
            OperationResult<int> result = await addRecipeService.AddRecipeManually(newRecipe);
            if (result.Success)
            {
                return Results.Ok($"Recipe ID {result} added successfully");
            }
            else
            {
                return Results.Problem("Recipe not added successfully");
            }
            
        });
    }
}