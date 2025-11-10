using savorfolio_backend.DataAccess;
using Tests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using Microsoft.VisualBasic;
using Tests.Helpers;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class IngredientListRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private readonly IngListRepository _repository = new(sqliteDbFixture.Context);
    private readonly RecipeRepository _recipeRepository = new(sqliteDbFixture.Context);
    private static readonly JObject _expectedViewRecipe;
    private static readonly JObject _expectedAddRecipe;

    static IngredientListRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");

        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }



    [Fact]
    public async Task GetIngredientsByIdTest()
    {
        // initialize test recipe ID
        int recipeId = 2;
        // initialize expected ingredient list as DTOs for case matching
        var expectedIngListDTO = _expectedViewRecipe["Ingredients"]?.ToObject<List<IngredientListDTO>>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedIngListDTO);
        JToken expectedIngList = JToken.Parse(expectedJson);

        // Call GetIngredientsByRecipeAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await _repository.GetIngredientsByRecipeAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedIngList, actualToken));
    }



    [Fact]
    public async Task AddNewIngredients()
    {
        // initialize the recipe DTO to add to the database
        var addRecipeDTO = _expectedAddRecipe["recipeSummary"]?.ToObject<RecipeDTO>();
        // initialize the list of ingredient DTOs to add to the table
        var ingList = _expectedAddRecipe["ingredients"]?.ToObject<List<IngredientListDTO>>();

        // initialize the number of records expected to be added to the table: 10
        int expectedRecordCount = 10;

        // call AddNewRecipeAsync with the DTO -- this is necessary to avoid foreign key violations
        var recipeId = await _recipeRepository.AddNewRecipeAsync(addRecipeDTO!);
        // call AddNewRecipeIng with the ingredient list and recipe ID
        int records = _repository.AddNewRecipeIng(ingList!, recipeId, sectionsData: []);

        // assert the expected record count is equal to the actual record count
        Assert.Equal(expectedRecordCount, records);
    }
}