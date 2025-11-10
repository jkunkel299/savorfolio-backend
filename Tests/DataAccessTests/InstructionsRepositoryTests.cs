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
    private readonly RecipeRepository _recipeRepository = new(sqliteDbFixture.Context);
    private static readonly JObject _expectedViewRecipe;
    
    private static readonly JObject _expectedAddRecipe;

    static IntructionsRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");

        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }



    [Fact]
    public async Task GetInstructionsByIdTest()
    {
        // initialize test recipe ID
        int recipeId = 2;
        // initialize expected instructions as a list of DTOs for case matching
        var expectedInsDTO = _expectedViewRecipe["Instructions"]?.ToObject<List<InstructionDTO>>();
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
    public async Task AddNewInstructions()
    {
        // initialize the recipe DTO to add to the database
        var addRecipeDTO = _expectedAddRecipe["recipeSummary"]?.ToObject<RecipeDTO>();
        // initialize the list of ingredient DTOs to add to the table
        var insList = _expectedAddRecipe["instructions"]?.ToObject<List<InstructionDTO>>();

        // initialize the number of records expected to be added to the table: 4
        int expectedRecordCount = 4;

        // call AddNewRecipeAsync with the DTO -- this is necessary to avoid foreign key violations
        var recipeId = await _recipeRepository.AddNewRecipeAsync(addRecipeDTO!);
        // call AddNewRecipeIng with the ingredient list and recipe ID
        int records = _repository.AddNewRecipeIns(insList!, recipeId, sectionsData: []);

        // assert the expected record count is equal to the actual record count
        Assert.Equal(expectedRecordCount, records);
    }
}