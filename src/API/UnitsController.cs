/* API layer for units-related functionality, e.g., search for unit */

using savorfolio_backend.Interfaces;

namespace savorfolio_backend.API;

public static class UnitEndpoints
{
    public static void MapUnitApi(this WebApplication app)
    {
        // get units by term
        app.MapGet(
            "/api/units",
            async (string term, IUnitsService unitsService) =>
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return Results.BadRequest("Search term is required.");
                }

                var results = await unitsService.SearchUnitsAsync(term);
                return Results.Ok(results);
            }
        );
    }
}
