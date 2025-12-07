using Microsoft.AspNetCore.Http;
using savorfolio_backend.Interfaces;

namespace Tests.Helpers;

public static class IngredientEndpointsHelper
{
    public static async Task<IResult> InvokeSearchEndpoint(string term, IIngredientService service)
    {
        // This replicates the API endpoint defined in IngredientEndpoints.MapIngredientApi
        if (string.IsNullOrWhiteSpace(term))
        {
            return Results.BadRequest("Search term is required.");
        }

        var results = await service.SearchIngredientsAsync(term);
        return Results.Ok(results);
    }
}
