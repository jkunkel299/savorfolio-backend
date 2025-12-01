using IntegrationTests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class RecipeSearchDbTests(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private static readonly List<RecipeDTO> _expectedRecipes;

    static RecipeSearchDbTests()
    {
        string recipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/RecipeDTOs.json");

        _expectedRecipes =
        [
            .. JsonToList.JsonFileToList<RecipeDTO>(recipeFilePath).OrderBy(r => r.Id),
        ];
    }

    [Fact]
    public async Task RecipeSearchDbEmpty()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // instantiate repository
        var recipeRepository = new RecipeRepository(_databaseFixture.Context);

        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with empty search term - should return all recipes
        var result = await recipeRepository.ReturnRecipesFiltered(request);
        var orderedResult = result.OrderBy(r => r.Id).ToList();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(orderedResult);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public async Task RecipeSearchDbIncludeIngredients()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // instantiate repository
        var recipeRepository = new RecipeRepository(_databaseFixture.Context);

        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { IncludeIngredients = [143] };

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes[0]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with include chicken filter - should return only chicken ragout
        var result = await recipeRepository.ReturnRecipesFiltered(request);

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(result[0]);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public async Task RecipeSearchDbExcludeIngredients()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // instantiate repository
        var recipeRepository = new RecipeRepository(_databaseFixture.Context);

        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { ExcludeIngredients = [143] };

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes[1]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with exclude chicken filter - should return only cookies
        var result = await recipeRepository.ReturnRecipesFiltered(request);

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(result[0]);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}
