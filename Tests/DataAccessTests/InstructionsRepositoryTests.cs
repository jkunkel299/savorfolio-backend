using savorfolio_backend.DataAccess;
using Tests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using Microsoft.VisualBasic;
using Tests.Helpers;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class IntructionsRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private readonly InstructionsRepository _repository = new(sqliteDbFixture.Context);
    private static readonly JObject _expectedViewRecipe;

    static IntructionsRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
    }



    [Fact]
    public async Task GetInstructionsByIdTest()
    {
        // initialize test recipe ID
        int recipeId = 2;
        // initialize expected instructions as a list of DTOs for case matching
        var expectedInsDTO = _expectedViewRecipe["instructions"]?.ToObject<List<InstructionDTO>>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedInsDTO);
        JToken expectedInsList = JToken.Parse(expectedJson);

        // Call GetInstructionsByRecipeAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await _repository.GetInstructionsByRecipeAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedInsList, actualToken));
    }

    [Fact]
    public void AddNewInstructions()
    {
        
    }
}