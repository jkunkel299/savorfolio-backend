using Moq;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;
using Newtonsoft.Json.Linq;
using Tests.Helpers;
using Newtonsoft.Json;

namespace Tests.LogicLayerTests;

public class AddRecipeServiceTests()
{
    private static readonly JObject _expectedAddRecipe;

    static AddRecipeServiceTests()
    {
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }



    // test that AddRecipeManually calls its dependent functions
    [Fact]
    public async Task AddRecipeManually_CallsFunctionsTest()
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipe;
        // initialize the recipe ID
        int recipeId = 3;
        // initialize sections (no sections)
        List<SectionDTO> sections = [];

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

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipe(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections))
                    .Returns(1);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections))
                    .Returns(1);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(1);
        mockSectionsRepo.Setup(r => r.AddNewRecipeSectionsAsync(It.IsAny<List<SectionDTO>>(), recipeId))
                    .ReturnsAsync((0, sections));

        // mock AddRecipeService
        var addRecipeService = new AddRecipeService(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // call AddRecipeManually from mocked AddRecipeService
        _ = await addRecipeService.AddRecipeManually(newRecipeContent);

        // assert mocked recipe repo function called once
        mockRecipeRepo.Verify(d => d.AddNewRecipe(It.IsAny<RecipeDTO>()), Times.AtMostOnce);
        // assert mocked ingredient list repo function called once
        mockIngListRepo.Verify(d => d.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections), Times.AtMostOnce);
        // assert mocked instructions repo function called once
        mockInstructionsRepo.Verify(d => d.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections), Times.AtMostOnce);
        // assert mocked tags repo function called once
        mockTagsRepo.Verify(d => d.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId), Times.AtMostOnce);
    }



    // test that AddRecipeManually returns the operation result successfully
    [Fact]
    public async Task AddRecipeManually_Success()
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipe;
        // initialize the recipe ID
        int recipeId = 3;
        // initialize sections (no sections)
        List<SectionDTO> sections = [];

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

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipe(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections))
                    .Returns(1);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections))
                    .Returns(1);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(1);

        // mock AddRecipeService
        var addRecipeService = new AddRecipeService(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // call AddRecipeManually from mocked AddRecipeService
        var result = await addRecipeService.AddRecipeManually(newRecipeContent);

        // assert result.success is true
        Assert.True(result.Success);
    }



    // test that AddRecipeManually returns an unsuccessful operation if the ingredients, instructions, or tags records added = 0
    [Theory]
    [InlineData(new int[] { 0, 1, 1 })]
    [InlineData(new int[] { 1, 0, 1 })]
    [InlineData(new int[] { 1, 1, 0 })]
    public async Task AddRecipeManuallyFailure(int[] returnValues)
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipe;
        // initialize the recipe ID
        int recipeId = 3;
        // initialize sections (no sections)
        List<SectionDTO> sections = [];

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

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipe(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections))
                    .Returns(returnValues[0]);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections))
                    .Returns(returnValues[1]);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(returnValues[2]);

        // mock AddRecipeService
        var addRecipeService = new AddRecipeService(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );

        // call AddRecipeManually from mocked AddRecipeService
        var result = await addRecipeService.AddRecipeManually(newRecipeContent);

        // assert result.success is false
        Assert.False(result.Success);
    }

}