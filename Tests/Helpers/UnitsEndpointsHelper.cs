using Microsoft.AspNetCore.Http;
using savorfolio_backend.Interfaces;

namespace Tests.Helpers;

public static class UnitsEndpointsHelper
{
    public static async Task<IResult> InvokeSearchEndpoint(string term, IUnitsService service)
    {
        // This replicates the API endpoint defined in UnitEndpoints.MapUnitApi
        if (string.IsNullOrWhiteSpace(term))
        {
            return Results.BadRequest("Search term is required.");
        }

        var results = await service.SearchUnitsAsync(term);
        return Results.Ok(results);
    }
}