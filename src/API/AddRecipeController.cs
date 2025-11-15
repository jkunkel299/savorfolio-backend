/* API layer to add recipe manually, may also include adding recipe via scraping */

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
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
            [Authorize]
            async (
                [FromBody] JsonDocument newRecipeBody,
                HttpContext httpContext,
                IAddRecipeService addRecipeService
            ) =>
            {
                var newRecipe = JObject.Parse(newRecipeBody.RootElement.GetRawText());
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = int.Parse(userIdClaim!);

                OperationResult<int> result = await addRecipeService.AddRecipeManuallyAsync(
                    newRecipe,
                    userId
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
