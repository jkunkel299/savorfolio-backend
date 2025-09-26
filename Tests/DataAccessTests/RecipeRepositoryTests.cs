using savorfolio_backend.DataAccess;
using Tests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using Microsoft.VisualBasic;
using Tests.Helpers;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class RecipeRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private readonly RecipeRepository _repository = new(sqliteDbFixture.Context);
    private static readonly List<RecipeDTO> _expectedRecipes;

    static RecipeRepositoryTests()
    {
        string recipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/RecipeDTOs.json");
        _expectedRecipes = [.. JsonToList.JsonFileToList<RecipeDTO>(recipeFilePath).OrderBy(r => r.Id)];
    }

    [Fact]
    public async Task RecipeSearchEmpty()
    {
        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // initialize expected result as string, convert to JSON
        List<RecipeDTO> recipesExpected = _expectedRecipes;
        string expectedJson = JsonConvert.SerializeObject(recipesExpected);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with empty search term - should return all recipes
        var result = await _repository.ReturnRecipesFiltered(request);
        var orderedResult = result.OrderBy(r => r.Id).ToList();

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(orderedResult);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }



    [Theory]
    // include chicken
    [InlineData(new int[] { 143 })]
    // include chicken and white cooking wine
    [InlineData(new int[] { 143, 2 })]
    // include semi-sweet chocolate chips
    [InlineData(new int[] { 38 })]
    // include pear (should not return matching recipes)
    [InlineData(new int[] { 61 })]
    public async Task RecipeSearchIncludeIngredients(int[] includeIngredients)
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [.. includeIngredients]
        };

        // manipulate _expectedRecipes to get appropriate expected
        var recipesExpected = _expectedRecipes
            .Where(r => request.IncludeIngredients.All(ingId =>
                r.Ingredients.Any(ri => ri.IngredientId == ingId)))
            .OrderBy(r => r.Id)
            .ToList();
        string expectedJson = JsonConvert.SerializeObject(recipesExpected);
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
    [InlineData(new int[] { 143 })]
    // exclude chicken and chocolate chips (should not return any recipes)
    [InlineData(new int[] { 143, 38 })]
    // exclude semi-sweet chocolate chips
    [InlineData(new int[] { 38 })]
    // exclude pear (should return both recipes)
    [InlineData(new int[] { 61 })]
    // [MemberData(nameof(RecipeSearchTestCasesExclude))]
    // [Fact]
    public async Task RecipeSearchExcludeIngredients(int[] excludeIngredients)
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [.. excludeIngredients]
        };

        // manipulate _expectedRecipes to get appropriate expected
        var recipesExpected = _expectedRecipes
            .Where(r => request.IncludeIngredients.All(ingId =>
                r.Ingredients.Any(ri => ri.IngredientId == ingId)))
            .OrderBy(r => r.Id)
            .ToList();
        // initialize convert expected result string to JSON
        string expectedJson = JsonConvert.SerializeObject(recipesExpected);
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