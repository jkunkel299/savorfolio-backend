using IntegrationTests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;
using Tests.TestData;

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

    #region Empty Search
    // database integration test for empty search
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
    #endregion

    #region Include Ingredients
    // Database integration for include ingredients
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
    #endregion

    #region Exclude Ingredients
    // Database integration for exclude ingredients
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
    #endregion

    #region Category Filter
    // Database integration for category filter
    [Theory]
    [MemberData(
        nameof(CategoryFilterData.CategoryFilterTestCases),
        MemberType = typeof(CategoryFilterData)
    )]
    public async Task RecipeSearchDbCategoryTags(RecipeFilterRequestDTO request)
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // instantiate repository
        var recipeRepository = new RecipeRepository(_databaseFixture.Context);

        // initialize expected recipe -
        // for recipe type and meal will be record 1,
        // for cuisine and dietary will be record 3
        RecipeDTO recipeExpected;
        if (request.Dietary != null || request.Cuisine != null)
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

        // Call ReturnRecipesFiltered with test case filter
        var result = await recipeRepository.ReturnRecipesFiltered(request);

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(result[0]);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
    #endregion

    #region Multiple Filters
    // Database integration for multiple filters
    [Theory]
    [MemberData(
        nameof(MultipleFiltersData.MultipleFiltersTestCases),
        MemberType = typeof(MultipleFiltersData)
    )]
    public async Task RecipeSearchMultipleFilters(int testCase, RecipeFilterRequestDTO request)
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // instantiate repository
        var recipeRepository = new RecipeRepository(_databaseFixture.Context);

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

        // initialize convert expected result string to JSON
        string expectedJson = JsonConvert.SerializeObject(recipeList);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with test case filter
        var result = await recipeRepository.ReturnRecipesFiltered(request);

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
    #endregion
}
