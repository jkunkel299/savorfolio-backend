using IntegrationTests.Fixtures;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class ViewRecipeDbTests(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private static readonly JObject _expectedViewRecipe;

    static ViewRecipeDbTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
    }



    [Fact]
    public async Task DbReturnRecipeById()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize test recipe Id
        int recipeId = 2;
        // instantiate repository
        var recipeRepository = new RecipeRepository(_databaseFixture.Context);

        // initialize expected result as "RecipeSummary" portion of JSON object _expectedViewRecipe
        var expectedRecipeDTO = _expectedViewRecipe["RecipeSummary"]?.ToObject<RecipeDTO>(); // converted to DTO for case sensitivity
        // convert to JSON Token
        var expectedJson = JsonConvert.SerializeObject(expectedRecipeDTO);
        JToken expectedRecipe = JToken.Parse(expectedJson);

        // Call ReturnRecipeByIdAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await recipeRepository.ReturnRecipeByIdAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedRecipe, actualToken));
    }



    [Fact]
    public async Task DbGetIngredientsById()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize test recipe ID
        int recipeId = 2;
        // instantiate repository
        var ingListRepository = new IngListRepository(_databaseFixture.Context);

        // initialize expected ingredient list as DTOs for case matching
        var expectedIngListDTO = _expectedViewRecipe["Ingredients"]?.ToObject<List<IngredientListDTO>>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedIngListDTO);
        JToken expectedIngList = JToken.Parse(expectedJson);

        // Call GetIngredientsByRecipeAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await ingListRepository.GetIngredientsByRecipeAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedIngList, actualToken));
    }



    [Fact]
    public async Task DbGetInstructionsById()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize test recipe ID
        int recipeId = 2;
        // instantiate repository
        var instructionsRepository = new InstructionsRepository(_databaseFixture.Context);

        // initialize expected instructions as a list of DTOs for case matching
        var expectedInsDTO = _expectedViewRecipe["Instructions"]?.ToObject<List<InstructionDTO>>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedInsDTO);
        JToken expectedInsList = JToken.Parse(expectedJson);

        // Call GetInstructionsByRecipeAsync with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = await instructionsRepository.GetInstructionsByRecipeAsync(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedInsList, actualToken));
    }



    [Fact]
    public void DbGetTagsById()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize test recipe ID
        int recipeId = 2;
        // instantiate repository
        var tagsRepository = new TagsRepository(_databaseFixture.Context);

        // initialize expected tags as a TagStringDTO for case matching
        var expectedTagsDTO = _expectedViewRecipe["RecipeTags"]?.ToObject<TagStringsDTO>();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedTagsDTO);
        JToken expectedTags = JToken.Parse(expectedJson);

        // Call GetTagsByRecipe with the expected recipe Id - should return Fall Spice Choc. Chip
        var result = tagsRepository.GetTagsByRecipe(recipeId);
        // Convert result to JSON Token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedTags, actualToken));
    }
}