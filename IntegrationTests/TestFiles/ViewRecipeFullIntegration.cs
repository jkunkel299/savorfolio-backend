using IntegrationTests.Fixtures;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class ViewRecipeFullTests(DatabaseFixture databaseFixture, TestServerFixture testServerFixture) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.HttpClient;
    private static readonly JObject _expectedViewRecipe;

    static ViewRecipeFullTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
    }


    [Fact]
    public async Task ViewRecipesTest()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize test recipe ID
        int recipeId = 2;

        // initialize expected result as DTO, convert to JSON token
        var expectedRecipeDTO = _expectedViewRecipe.ToObject<FullRecipeDTO>();
        string expectedJson = JsonConvert.SerializeObject(expectedRecipeDTO);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with empty search filter - should return all recipes
        var response = await _client.GetAsync($"/api/recipes/view?recipeId={recipeId}");
        // get response content
        var recipe = await response.Content.ReadFromJsonAsync<FullRecipeDTO>();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(recipe);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(recipe);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}