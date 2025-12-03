using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Models.DTOs;
using Tests.Fixtures;
using Tests.Helpers;
using Tests.TestData;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class RecipeRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private readonly RecipeRepository _repository = new(sqliteDbFixture.Context);
    private static readonly List<RecipeDTO> _expectedFilteredRecipes;
    private static readonly JObject _expectedViewRecipe;
    private static readonly JObject _expectedAddRecipe;

    static RecipeRepositoryTests()
    {
        string recipeFilteringFilePath = TestFileHelper.GetProjectPath(
            "ExpectedData/RecipeDTOs.json"
        );
        string viewRecipeFilePath = TestFileHelper.GetProjectPath(
            "ExpectedData/ViewRecipeDTO.json"
        );
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");

        _expectedFilteredRecipes =
        [
            .. JsonToList.JsonFileToList<RecipeDTO>(recipeFilteringFilePath).OrderBy(r => r.Id),
        ];
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }

    #region Empty Search
    [Fact]
    public async Task RecipeSearchEmpty()
    {
        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // initialize expected result as string, convert to JSON
        List<RecipeDTO> recipesExpected = _expectedFilteredRecipes;
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
    #endregion

    #region Include Ingredients
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
        var request = new RecipeFilterRequestDTO { IncludeIngredients = [.. includeIngredients] };

        // manipulate _expectedRecipes to get appropriate expected
        var recipesExpected = _expectedFilteredRecipes
            .Where(r =>
                request.IncludeIngredients.All(ingId =>
                    r.Ingredients.Any(ri => ri.IngredientId == ingId)
                )
            )
            .OrderBy(r => r.Id)
            .ToList();
        string expectedJson = JsonConvert.SerializeObject(recipesExpected);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with include ingredient
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
    #endregion

    #region Exclude Ingredients
    [Theory]
    // exclude chicken
    [InlineData(new int[] { 143 })]
    // exclude chicken and chocolate chips (should not return any recipes)
    [InlineData(new int[] { 143, 38 })]
    // exclude semi-sweet chocolate chips
    [InlineData(new int[] { 38 })]
    // exclude pear (should return both recipes)
    [InlineData(new int[] { 61 })]
    public async Task RecipeSearchExcludeIngredients(int[] excludeIngredients)
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { ExcludeIngredients = [.. excludeIngredients] };

        var ingredientIds = request.ExcludeIngredients;

        // manipulate _expectedRecipes to get appropriate expected
        var recipesExpected = _expectedFilteredRecipes
            .Where(r =>
                !ingredientIds.All(ingId => r.Ingredients.Any(ri => ri.IngredientId == ingId))
            )
            .OrderBy(r => r.Id)
            .ToList();
        // initialize convert expected result string to JSON
        string expectedJson = JsonConvert.SerializeObject(recipesExpected);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered with exclude ingredient
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
    #endregion

    #region Category Filters
    [Theory]
    [MemberData(
        nameof(CategoryFilterData.CategoryFilterTestCases),
        MemberType = typeof(CategoryFilterData)
    )]
    public async Task RecipeSearchCategoryTags(RecipeFilterRequestDTO request)
    {
        // initialize expected recipe -
        // for recipe type and meal will be record 1,
        // for cuisine and dietary will be record 3
        RecipeDTO recipeExpected;
        if (request.Dietary != null || request.Cuisine != null)
        {
            recipeExpected = _expectedFilteredRecipes[2];
        }
        else
        {
            recipeExpected = _expectedFilteredRecipes[0];
        }
        List<RecipeDTO> recipeList = [recipeExpected];

        // initialize convert expected result string to JSON
        string expectedJson = JsonConvert.SerializeObject(recipeList);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
    #endregion

    #region Multiple Filters
    [Theory]
    [MemberData(
        nameof(MultipleFiltersData.MultipleFiltersTestCases),
        MemberType = typeof(MultipleFiltersData)
    )]
    public async Task RecipeSearchMultipleFilters(int testCase, RecipeFilterRequestDTO request)
    {
        // initialize expected recipe -
        // test case 1 -> record 2
        // test case 2 -> record 3
        // test case 3 -> record 3
        RecipeDTO recipeExpected;
        if (testCase == 1)
        {
            recipeExpected = _expectedFilteredRecipes[1];
        }
        else
        {
            recipeExpected = _expectedFilteredRecipes[2];
        }
        List<RecipeDTO> recipeList = [recipeExpected];

        // initialize convert expected result string to JSON
        string expectedJson = JsonConvert.SerializeObject(recipeList);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call ReturnRecipesFiltered
        var result = await _repository.ReturnRecipesFiltered(request);

        // Convert result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
    #endregion

    #region Get Recipe by ID
    [Fact]
    public async Task ReturnRecipeById()
    {
        // initialize test recipe Id
        int recipeId = 2;
        // initialize expected result as "RecipeSummary" portion of JSON object _expectedViewRecipe
        var expectedRecipeDTO = _expectedViewRecipe["RecipeSummary"]?.ToObject<RecipeDTO>(); // converted to DTO for case sensitivity
        // convert to JSON Token
        var expectedJson = JsonConvert.SerializeObject(expectedRecipeDTO);
        JToken expectedRecipe = JToken.Parse(expectedJson);

        // Call ReturnRecipeByIdAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await _repository.ReturnRecipeByIdAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedRecipe, actualToken));
    }
    #endregion

    #region Add Recipe
    [Fact]
    public async Task AddNewRecipeTest()
    {
        // initialize expected recipe ID
        int expectedRecipeId = 4;

        // initialize the recipe DTO to add to the database
        var addRecipeDTO = _expectedAddRecipe["recipeSummary"]?.ToObject<RecipeDTO>();

        // call AddNewRecipe with the DTO
        var resultId = await _repository.AddNewRecipeAsync(addRecipeDTO!);

        // assert the expected recipe ID is equal to the added recipe ID
        Assert.Equal(expectedRecipeId, resultId);
    }
    #endregion
}
