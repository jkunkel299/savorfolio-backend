/* API Layer for getting enums to frontend */
using savorfolio_backend.Models.enums;
using static savorfolio_backend.Utils.EnumExtensions;

namespace savorfolio_backend.API;

public static class TagsEndpoints
{    
    public static void GetMealTags(this WebApplication app)
    {
        app.MapGet("api/enum/meal-type", () =>
        {
            var values = Enum.GetNames<MealTag>();
            return Results.Ok(values);
        });

    }

    public static void GetRecipeTypeTags(this WebApplication app)
    {
        app.MapGet("api/enum/recipe-type", () =>
        {
            var values = Enum.GetNames<RecipeTypeTag>();
            return Results.Ok(values);
        });

    }

    public static void GetCuisineTags(this WebApplication app)
    {
        app.MapGet("api/enum/cuisine-type", () =>
        {
            var values = Enum.GetNames<CuisineTag>();
            return Results.Ok(values);
        });

    }

    public static void GetDietaryTags(this WebApplication app)
    {
        app.MapGet("api/enum/dietary-type", () =>
        {
            var values = GetEnumList<DietaryTag>();
            return Results.Ok(values);
        });

    }
}