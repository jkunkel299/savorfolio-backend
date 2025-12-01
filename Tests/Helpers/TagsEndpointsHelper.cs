using Microsoft.AspNetCore.Http;
using savorfolio_backend.Models.enums;
using static savorfolio_backend.Utils.EnumExtensions;

namespace Tests.Helpers;

public static class TagsEndpointsHelper
{
    public static IResult InvokeGetMealTagsEndpoint()
    {
        // This replicates the API endpoint defined in TagsEndpoints.GetMealTags
        var values = Enum.GetNames<MealTag>();
        return Results.Ok(values);
    }

    public static IResult InvokeGetRecipeTypeTagsEndpoint()
    {
        // This replicates the API endpoint defined in TagsEndpoints.GetRecipeTypeTags
        var values = Enum.GetNames<RecipeTypeTag>();
        return Results.Ok(values);
    }

    public static IResult InvokeGetCuisineTagsEndpoint()
    {
        // This replicates the API endpoint defined in TagsEndpoints.GetCuisineTags
        var values = Enum.GetNames<CuisineTag>();
        return Results.Ok(values);
    }

    public static IResult InvokeGetDietaryTagsEndpoint()
    {
        // This replicates the API endpoint defined in TagsEndpoints.GetDietaryTags
        var values = GetEnumList<DietaryTag>();
        return Results.Ok(values);
    }
}
