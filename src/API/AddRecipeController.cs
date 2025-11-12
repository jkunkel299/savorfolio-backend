/* API layer to add recipe manually, may also include adding recipe via scraping */

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.API;

public static class AddRecipeEndpoints
{
    public static void MapManualRecipe(this WebApplication app)
    {
        app.MapPost(
            "/api/recipes/add/manual",
            async ([FromBody] JsonDocument newRecipeBody, IAddRecipeService addRecipeService) =>
            {
                var newRecipe = JObject.Parse(newRecipeBody.RootElement.GetRawText());

                OperationResult<int> result = await addRecipeService.AddRecipeManuallyAsync(
                    newRecipe
                );
                if (result.Success)
                {
                    return Results.Ok($"Recipe ID {result.Data} added successfully");
                }
                else
                {
                    return Results.Problem("Recipe not added successfully");
                }
            }
        );
    }

    public static void MapDraftRecipe(this WebApplication app)
    {
        app.MapPost(
            "/api/recipes/add/scrape",
            async ([FromBody] string url, IWebScraperService webScraperService) =>
            {
                DraftRecipeDTO results = await webScraperService.RunScraperAsync(url);
                return Results.Ok(results);
            }
        );
    }
}
