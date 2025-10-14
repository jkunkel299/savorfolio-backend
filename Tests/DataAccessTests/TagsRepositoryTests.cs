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
    private static readonly JObject _expectedViewRecipe;

    static TagsRepositoryTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
    }



    [Fact]
    public void GetTagsByRecipeTest()
    {
        // initialize test recipe ID
        int recipeId = 2;
        // initialize expected tags as a TagStringDTO for case matching
        var expectedTagsDTO = _expectedViewRecipe["recipeTags"]?.ToObject<TagStringsDTO>();
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
    public void AddTagsTest()
    {
        
    }
}