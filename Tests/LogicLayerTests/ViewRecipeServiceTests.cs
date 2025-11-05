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
        // mock sections repository interface
        var mockSectionsRepo = new Mock<ISectionsRepository>();
        // mock ViewRecipeService
        var viewRecipeService = new ViewRecipeService(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // call CompileRecipeAsync from mocked ViewRecipeService
        _ = await viewRecipeService.CompileRecipeAsync(recipeId);

        // assert mocked recipe repo function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipeByIdAsync(recipeId), Times.AtMostOnce);
        // assert mocked ingredient list repo function called once
        mockIngListRepo.Verify(d => d.GetIngredientsByRecipeAsync(recipeId), Times.AtMostOnce);
        // assert mocked instructions repo function called once
        mockInstructionsRepo.Verify(d => d.GetInstructionsByRecipeAsync(recipeId), Times.AtMostOnce);
        // assert mocked tags repo function called once
        mockTagsRepo.Verify(d => d.GetTagsByRecipe(recipeId), Times.AtMostOnce);
    }



    [Fact]
    public async Task CompileRecipesAsyncCreatesDTO()
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
        // mock sections repository interface
        var mockSectionsRepo = new Mock<ISectionsRepository>();
        // mock ViewRecipeService
        var viewRecipeService = new ViewRecipeService(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // set up expected DTO return from recipe repository
        var mockRecipeSummary = _expectedViewRecipe["RecipeSummary"]?.ToObject<RecipeDTO>() ?? new RecipeDTO();
        mockRecipeRepo.Setup(d => d.ReturnRecipeByIdAsync(recipeId))
                        .ReturnsAsync(mockRecipeSummary);
        // set up expected DTO return from sections repository (no sections)
        var mockSections = _expectedViewRecipe["RecipeSections"]?.ToObject<List<SectionDTO>>() ?? [];
        // set up expected DTO return from ingredient list repository
        var mockIngList = _expectedViewRecipe["Ingredients"]?.ToObject<List<IngredientListDTO>>() ?? [];
        mockIngListRepo.Setup(d => d.GetIngredientsByRecipeAsync(recipeId))
                        .ReturnsAsync(mockIngList);
        // set up expected DTO return from instructions repository
        var mockInsList = _expectedViewRecipe["Instructions"]?.ToObject<List<InstructionDTO>>() ?? [];
        mockInstructionsRepo.Setup(d => d.GetInstructionsByRecipeAsync(recipeId))
                        .ReturnsAsync(mockInsList);
        // set up expected DTO return from tags repository
        var mockTags = _expectedViewRecipe["RecipeTags"]?.ToObject<TagStringsDTO>() ?? new TagStringsDTO();
        mockTagsRepo.Setup(d => d.GetTagsByRecipe(recipeId))
                        .Returns(mockTags);

        // initialize expected FullRecipeDTO return
        FullRecipeDTO expectedReturn = new()
        {
            RecipeId = recipeId,
            RecipeSummary = mockRecipeSummary,
            RecipeTags = mockTags,
            RecipeSections = mockSections,
            Ingredients = mockIngList,
            Instructions = mockInsList
        };
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedReturn);
        JToken expectedFullRecipe = JToken.Parse(expectedJson);

        // call CompileRecipesAsync with the test recipe ID
        var result = await viewRecipeService.CompileRecipeAsync(recipeId);
        // convert to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedFullRecipe, actualToken));
    }
}