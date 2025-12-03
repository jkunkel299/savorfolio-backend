using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Models.enums;

namespace Tests.TestData;

public class MultipleFiltersData
{
    public static TheoryData<int, RecipeFilterRequestDTO> MultipleFiltersTestCases() =>
        new()
        {
            { // include ingredients (semi-sweet chocolate chips) and tags (recipe type=dessert)
                1,
                new RecipeFilterRequestDTO
                {
                    IncludeIngredients = [38],
                    Recipe_type = DupEnumExtensions.ParseEnumMember<RecipeTypeTag>("Dessert"),
                }
            },
            { // include (brown sugar) and exclude (semi-sweet chocolate chips) ingredients
                2,
                new RecipeFilterRequestDTO { IncludeIngredients = [200], ExcludeIngredients = [38] }
            },
            { // multiple tags (Cuisine=American and Recipe type=dessert)
                3,
                new RecipeFilterRequestDTO
                {
                    Cuisine = DupEnumExtensions.ParseEnumMember<CuisineTag>("American"),
                    Recipe_type = DupEnumExtensions.ParseEnumMember<RecipeTypeTag>("Dessert"),
                }
            },
        };
}
