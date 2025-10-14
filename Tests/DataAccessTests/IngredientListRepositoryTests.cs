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
    private static readonly JObject _expectedViewRecipe;

    static IngredientListRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
    }



    [Fact]
    public async Task GetIngredientsByIdTest()
    {
        // initialize test recipe ID
        int recipeId = 2;
        // initialize expected ingredient list as DTOs for case matching
        var expectedIngListDTO = _expectedViewRecipe["ingredients"]?.ToObject<List<IngredientListDTO>>();
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
    public void AddNewIngredients()
    {
        
    }
}