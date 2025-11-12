/* API layer for ingredient variant-related functionality, e.g., search for ingredients */

using savorfolio_backend.Interfaces;

namespace savorfolio_backend.API;

public static class IngredientEndpoints
{
    public static void MapIngredientApi(this WebApplication app)
    {
        // get ingredients by term
        app.MapGet(
            "/api/ingredients/search",
            async (string term, IIngredientService ingredientService) =>
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return Results.BadRequest("Search term is required.");
                }

                var results = await ingredientService.SearchIngredientsAsync(term);
                return Results.Ok(results);
            }
        );
    }
}
