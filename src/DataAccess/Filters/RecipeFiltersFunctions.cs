using savorfolio_backend.Models;

namespace savorfolio_backend.DataAccess.Filters;

public static class RecipeFilters
{
    public static IQueryable<Recipe> FilterByIngredients(this IQueryable<Recipe> query, List<int>? ingredientIds)
    {
        if (ingredientIds == null || ingredientIds.Count == 0)
            return query;

        return from r in query
               where r.IngredientLists
                   .Select(ri => ri.IngredientId)
                   .Distinct()
                   .Intersect(ingredientIds)
                   .Count() == ingredientIds.Count
               select r;

        // var ids = ingredientIds.ToArray(); // EF can handle arrays better
        // return query.Where(r =>
        //     ids.All(ingId =>
        //         r.IngredientLists.Any(ri => ri.IngredientId == ingId)));


        // if (ingredientIds == null || ingredientIds.Count == 0)
        //     return query;

        // return query = query.Where(r =>
        //         ingredientIds.All(ingId =>
        //             r.IngredientLists.Any(ri => ri.IngredientId == ingId)));
    }
    
    // additional functions to filter by cuisine, dietary restriction, etc.
}