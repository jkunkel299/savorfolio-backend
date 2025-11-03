using savorfolio_backend.DataAccess;
using Tests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;
using Microsoft.VisualBasic;
using Tests.Helpers;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class TagsRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private readonly TagsRepository _repository = new(sqliteDbFixture.Context);
    private readonly RecipeRepository _recipeRepository = new(sqliteDbFixture.Context);
    private static readonly JObject _expectedViewRecipe;
    private static readonly JObject _expectedAddRecipe;

    static TagsRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");

        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }



    [Fact]
    public void GetTagsByRecipeTest()
    {
        // initialize test recipe ID
        int recipeId = 2;
        // initialize expected tags as a TagStringDTO for case matching
        var expectedTagsDTO = _expectedViewRecipe["RecipeTags"]?.ToObject<TagStringsDTO>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedTagsDTO);
        JToken expectedTags = JToken.Parse(expectedJson);

        // Call GetTagsByRecipe with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = _repository.GetTagsByRecipe(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedTags, actualToken));
    }



    [Fact]
    public async Task AddTagsTest()
    {
        // initialize the recipe DTO to add to the database
        var addRecipeDTO = _expectedAddRecipe["recipeSummary"]?.ToObject<RecipeDTO>();
        // initialize the list of ingredient DTOs to add to the table
        var tags = _expectedAddRecipe["recipeTags"]?.ToObject<RecipeTagDTO>();

        // initialize the number of records expected to be added to the table: 1
        int expectedRecordCount = 1;

        // call AddNewRecipe with the DTO -- this is necessary to avoid foreign key violations
        var recipeId = await _recipeRepository.AddNewRecipe(addRecipeDTO!);
        // call AddNewRecipeIng with the ingredient list and recipe ID
        int records = _repository.AddNewRecipeTags(tags!, recipeId);

        // assert the expected record count is equal to the actual record count
        Assert.Equal(expectedRecordCount, records);
    }
}