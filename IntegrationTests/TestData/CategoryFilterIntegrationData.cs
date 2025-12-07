using savorfolio_backend.Models.DTOs;

namespace IntegrationTests.TestData;

public class CategoryFilterIntegrationData
{
    public static TheoryData<int, RecipeFilterRequestDTO> CategoryFilterIntegrationTestCases() =>
        new()
        {
            {
                1,
                new RecipeFilterRequestDTO { Recipe_typeString = "Main" }
            },
            {
                2,
                new RecipeFilterRequestDTO { MealString = "Dinner" }
            },
            {
                3,
                new RecipeFilterRequestDTO { CuisineString = "American" }
            },
            {
                4,
                new RecipeFilterRequestDTO { Dietary = ["Soy-Free"] }
            },
        };
}
