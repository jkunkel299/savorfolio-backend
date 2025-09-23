using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using savorfolio_backend.Interfaces;

namespace Tests.Helpers;

public static class IngredientEndpointsTestHelper
{
    public static async Task<IResult> InvokeSearchEndpoint(string term, IIngredientService service)
        {
        // This replicates the API endpoint defined in IngredientEndpoints.MapIngredientApi
        static async Task<IResult> endpointDelegate(string t, IIngredientService ingredientService)
            {
                if (string.IsNullOrWhiteSpace(t))
                {
                    return Results.BadRequest("Search term is required.");
                }

                var results = await ingredientService.SearchIngredientsAsync(t);
                return Results.Ok(results);
            }

            // Call the local delegate
            return await endpointDelegate(term, service);
        }

    // Convert IResult to IActionResult for xUnit assertions
    public static IActionResult ToActionResult(IResult result)
    {
        return result switch
        {
            // Results.Ok returns an OkObjectResult
            OkObjectResult ok => ok,
            // Results.BadRequest returns a BadRequestObjectResult
            BadRequestObjectResult bad => bad,
            // If you add more result types, expand this
            _ => throw new InvalidOperationException("Unsupported IResult type")
        };
    }
}