using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Fixtures;
using IntegrationTests.TestData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;
using Tests.TestData;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class RecipeSearchFullTests(
    DatabaseFixture databaseFixture,
    TestServerFixture testServerFixture
) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.AuthenticatedClient;
    private static readonly List<RecipeDTO> _expectedRecipes;

    static RecipeSearchFullTests()
    {
        string recipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/RecipeDTOs.json");

        _expectedRecipes =
        [
            .. JsonToList.JsonFileToList<RecipeDTO>(recipeFilePath).OrderBy(r => r.Id),
        ];
    }

    #region Empty Search
    // integration test for empty recipe search
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
    #endregion

    #region Include Ingredients
    // integration test for recipe search include ingredients
    [Fact]
    public async Task RecipeSearchIncludeIngredients()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { IncludeIngredients = [143] };

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes[0]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with filter
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
    #endregion

    #region Exclude Ingredients
    // integration test for recipe search exclude ingredients
    [Fact]
    public async Task RecipeSearchExcludeIngredients()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { ExcludeIngredients = [143] };

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(_expectedRecipes[1]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with filter
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
    #endregion

    #region Category Filtering
    // integration test for recipe search category filtering
    [Theory]
    [MemberData(
        nameof(CategoryFilterIntegrationData.CategoryFilterIntegrationTestCases),
        MemberType = typeof(CategoryFilterIntegrationData)
    )]
    public async Task RecipeSearchCategoryTags(int testCase, RecipeFilterRequestDTO request)
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize expected recipe -
        // for recipe type and meal will be record 1,
        // for cuisine and dietary will be record 3
        RecipeDTO recipeExpected;
        if (testCase == 4 || testCase == 3)
        {
            recipeExpected = _expectedRecipes[2];
        }
        else
        {
            recipeExpected = _expectedRecipes[0];
        }
        List<RecipeDTO> recipeList = [recipeExpected];

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(recipeList[0]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with test case filter
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
    #endregion

    #region Multiple Criteria
    // integration test for recipe search multiple simultaneous filters
    [Theory]
    [MemberData(
        nameof(MultipleFiltersData.MultipleFiltersTestCases),
        MemberType = typeof(MultipleFiltersData)
    )]
    public async Task RecipeSearchMultipleFilters(int testCase, RecipeFilterRequestDTO request)
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize expected recipe -
        // test case 1 -> record 2
        // test case 2 -> record 3
        // test case 3 -> record 3
        RecipeDTO recipeExpected;
        if (testCase == 1)
        {
            recipeExpected = _expectedRecipes[1];
        }
        else
        {
            recipeExpected = _expectedRecipes[2];
        }
        List<RecipeDTO> recipeList = [recipeExpected];

        // initialize expected result as string, convert to JSON token
        string expectedJson = JsonConvert.SerializeObject(recipeList[0]);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call MapRecipeSearch API with test case filter
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
    #endregion
}
