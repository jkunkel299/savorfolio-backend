using Moq;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;
using Newtonsoft.Json.Linq;
using Tests.Helpers;
using Newtonsoft.Json;

namespace Tests.LogicLayerTests;

public class ViewRecipeServiceTests()
{
    private static readonly JObject _expectedViewRecipe;

    static ViewRecipeServiceTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/ViewRecipeDTO.json");
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
    }



    // test that CompileRecipeAsync calls its dependent functions
    [Fact]
    public async Task CompileRecipeAsync_CallsFunctionsTest()
    {
        // initialize test recipe ID
        int recipeId = 2;

        // mock recipe repository interface
        var mockRecipeRepo = new Mock<IRecipeRepository>();
        // mock ingredient list repository interface
        var mockIngListRepo = new Mock<IIngListRepository>();
        // mock instructions repository interface
        var mockInstructionsRepo = new Mock<IInstructionsRepository>();
        // mock tags repository interface
        var mockTagsRepo = new Mock<ITagsRepository>();
        // mock ViewRecipeService
        var viewRecipeService = new ViewRecipeService(
            mockRecipeRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // call CompileRecipeAsync from mocked RecipeService
        _ = await viewRecipeService.CompileRecipeAsync(recipeId);

        // assert mocked recipe repo function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipeByIdAsync(recipeId), Times.Once);
        // assert mocked ingredient list repo function called once
        mockIngListRepo.Verify(d => d.GetIngredientsByRecipeAsync(recipeId), Times.Once);
        // assert mocked instructions repo function called once
        mockInstructionsRepo.Verify(d => d.GetInstructionsByRecipeAsync(recipeId), Times.Once);
        // assert mocked tags repo function called once
        mockTagsRepo.Verify(d => d.GetTagsByRecipe(recipeId), Times.Once);
    }



    [Fact]
    public async Task CompileRecipesAsync_CreatesDTO()
    {
        // initialize test recipe ID
        int recipeId = 2;

        // mock recipe repository interface
        var mockRecipeRepo = new Mock<IRecipeRepository>();
        // mock ingredient list repository interface
        var mockIngListRepo = new Mock<IIngListRepository>();
        // mock instructions repository interface
        var mockInstructionsRepo = new Mock<IInstructionsRepository>();
        // mock tags repository interface
        var mockTagsRepo = new Mock<ITagsRepository>();
        // mock ViewRecipeService
        var viewRecipeService = new ViewRecipeService(
            mockRecipeRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // set up expected DTO return from recipe repository
        var mockRecipeSummary = _expectedViewRecipe["recipeSummary"]?.ToObject<RecipeDTO>() ?? new RecipeDTO();
        mockRecipeRepo.Setup(d => d.ReturnRecipeByIdAsync(recipeId))
                        .ReturnsAsync(mockRecipeSummary);
        // set up expected DTO return from ingredient list repository
        var mockIngList = _expectedViewRecipe["ingredients"]?.ToObject<List<IngredientListDTO>>() ?? [];
        mockIngListRepo.Setup(d => d.GetIngredientsByRecipeAsync(recipeId))
                        .ReturnsAsync(mockIngList);
        // set up expected DTO return from instructions repository
        var mockInsList = _expectedViewRecipe["instructions"]?.ToObject<List<InstructionDTO>>() ?? [];
        mockInstructionsRepo.Setup(d => d.GetInstructionsByRecipeAsync(recipeId))
                        .ReturnsAsync(mockInsList);
        // set up expected DTO return from tags repository
        var mockTags = _expectedViewRecipe["recipeTags"]?.ToObject<TagStringsDTO>() ?? new TagStringsDTO();
        mockTagsRepo.Setup(d => d.GetTagsByRecipe(recipeId))
                        .Returns(mockTags);

        // initialize expected FullRecipeDTO return
        FullRecipeDTO expectedReturn = new()
        {
            RecipeId = recipeId,
            RecipeSummary = mockRecipeSummary,
            RecipeTags = mockTags,
            Ingredients = mockIngList,
            Instructions = mockInsList
        };
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedReturn);
        JToken expectedFullRecipe = JToken.Parse(expectedJson);

        // call CompileRecipesAsync with the test recipe ID
        var result = viewRecipeService.CompileRecipeAsync(recipeId);
        // convert to JSON
        var actualJson = JsonConvert.SerializeObject(result?.Result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedFullRecipe, actualToken));
    }
}