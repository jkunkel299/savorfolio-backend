using IntegrationTests.Fixtures;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class RecipeSearchFullTests(DatabaseFixture databaseFixture, TestServerFixture testServerFixture) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.HttpClient;
    private static readonly List<RecipeDTO> _expectedRecipes;

    static RecipeSearchFullTests()
    {
        string recipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/RecipeDTOs.json");

        _expectedRecipes = [.. JsonToList.JsonFileToList<RecipeDTO>(recipeFilePath).OrderBy(r => r.Id)];
    }

    [Fact]
    public async Task RecipeSearchEmpty()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with empty search filter - should return all recipes
        var response = await _client.PostAsJsonAsync("/api/recipes/search", request);
        // get response content
        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeDTO>>();
        // ensure response content is in expected order for assertion
        var orderedResult = recipes!.OrderBy(r => r.Id).ToList();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(orderedResult);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(recipes);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public async Task RecipeSearchIncludeIngredients()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [143]
        };

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes[0]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with empty search filter - should return all recipes
        var response = await _client.PostAsJsonAsync("/api/recipes/search", request);
        // get response content
        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(recipes![0]);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(recipes);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public async Task RecipeSearchExcludeIngredients()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            ExcludeIngredients = [143]
        };

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes[1]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with empty search filter - should return all recipes
        var response = await _client.PostAsJsonAsync("/api/recipes/search", request);
        // get response content
        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeDTO>>();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(recipes![0]);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(recipes);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}