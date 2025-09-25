using savorfolio_backend.DataAccess;
using Tests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace Tests.DataAccessTests;

[Collection("Unit test database Collection")]
public class RecipeRepositoryTests(UnitDbFixture unitDbFixture) : IClassFixture<UnitDbFixture>
{
    private readonly RecipeRepository _repository = new(unitDbFixture.Context);

    [Fact]
    public async Task RecipeSearchEmpty()
    {
        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        [
            {"Name":"Chicken Ragout","Servings":4,"CookTime":"20 minutes","PrepTime":"10 minutes","BakeTemp":null,"Temp_unit":null},
            {"Name":"Fall Spice Chocolate Chip Cookies","Servings":8,"CookTime":"10 minutes","PrepTime":"15 minutes","BakeTemp":400,"Temp_unit":"F"}
        ]
        """;
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with empty search term - should return all recipes
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }



    [Theory]
    // include chicken
    [InlineData(new int[] { 143 }, """
    [
        {"Name":"Chicken Ragout","Servings":4,"CookTime":"20 minutes","PrepTime":"10 minutes","BakeTemp":null,"Temp_unit":null},
    ]
    """)]
    // include chicken and white cooking wine
    [InlineData(new int[] { 143, 2 }, """
    [
        {"Name":"Chicken Ragout","Servings":4,"CookTime":"20 minutes","PrepTime":"10 minutes","BakeTemp":null,"Temp_unit":null},
    ]
    """)]
    // include semi-sweet chocolate chips
    [InlineData(new int[] { 38 }, """
    [
        {"Name":"Fall Spice Chocolate Chip Cookies","Servings":8,"CookTime":"10 minutes","PrepTime":"15 minutes","BakeTemp":400,"Temp_unit":"F"}
    ]
    """)]
    // include pear (should not return matching recipes)
    [InlineData(new int[] { 61 }, """[]""")]
    public async Task RecipeSearchIncludeIngredients(int[] includeIngredients, string expectedJson)
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [.. includeIngredients]
        };

        // initialize convert expected result string to JSON
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with empty search term - should return all recipes
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }



    [Theory]
    // exclude chicken
    [InlineData(new int[] { 143 }, """
    [
        {"Name":"Fall Spice Chocolate Chip Cookies","Servings":8,"CookTime":"10 minutes","PrepTime":"15 minutes","BakeTemp":400,"Temp_unit":"F"}
    ]
    """)]
    // exclude chicken and chocolate chips (should not return any recipes)
    [InlineData(new int[] { 143, 2 }, """[]""")]
    // exclude semi-sweet chocolate chips
    [InlineData(new int[] { 38 }, """
    [
        {"Name":"Chicken Ragout","Servings":4,"CookTime":"20 minutes","PrepTime":"10 minutes","BakeTemp":null,"Temp_unit":null}
    ]
    """)]
    // exclude pear (should return both recipes)
    [InlineData(new int[] { 61 }, """
    [
        {"Name":"Chicken Ragout","Servings":4,"CookTime":"20 minutes","PrepTime":"10 minutes","BakeTemp":null,"Temp_unit":null},
        {"Name":"Fall Spice Chocolate Chip Cookies","Servings":8,"CookTime":"10 minutes","PrepTime":"15 minutes","BakeTemp":400,"Temp_unit":"F"}
    ]
    """)]
    public async Task RecipeSearchExcludeIngredients(int[] excludeIngredients, string expectedJson)
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [.. excludeIngredients]
        };

        // initialize convert expected result string to JSON
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with empty search term - should return all recipes
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}