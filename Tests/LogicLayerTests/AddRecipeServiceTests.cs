using Moq;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;
using Newtonsoft.Json.Linq;
using Tests.Helpers;

namespace Tests.LogicLayerTests;

public class AddRecipeServiceTests()
{
    private static readonly JObject _expectedAddRecipe;
    private static readonly JObject _expectedAddRecipeSections;
    // mock recipe repository interface
    private static readonly Mock<IRecipeRepository> mockRecipeRepo = new();
    // mock ingredient list repository interface
    private static readonly Mock<IIngListRepository> mockIngListRepo = new();
    // mock instructions repository interface
    private static readonly Mock<IInstructionsRepository> mockInstructionsRepo = new();
    // mock tags repository interface
    private static readonly Mock<ITagsRepository> mockTagsRepo = new();
    // mock sections repository interface
    private static readonly Mock<ISectionsRepository> mockSectionsRepo = new();
    // mock AddRecipeService
    private static readonly AddRecipeService addRecipeService;

    static AddRecipeServiceTests()
    {
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");
        string addRecipeSectionsFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
        _expectedAddRecipeSections = JObject.Parse(File.ReadAllText(addRecipeSectionsFilePath));

        addRecipeService = new(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );
    }



    // test that AddRecipeManually calls its dependent functions
    [Fact]
    public async Task AddRecipeManually_CallsFunctionsTest()
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipe;
        // initialize the recipe ID
        int recipeId = 4;
        // initialize sections (no sections)
        List<SectionDTO> sections = [];

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipeAsync(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockSectionsRepo.Setup(r => r.AddNewRecipeSectionsAsync(It.IsAny<List<SectionDTO>>(), recipeId))
                    .ReturnsAsync((0, sections));
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections))
                    .Returns(1);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections))
                    .Returns(1);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(1);

        // call AddRecipeManually from mocked AddRecipeService
        _ = await addRecipeService.AddRecipeManuallyAsync(newRecipeContent);

        // assert mocked recipe repo function called once
        mockRecipeRepo.Verify(d => d.AddNewRecipeAsync(It.IsAny<RecipeDTO>()), Times.AtMostOnce);
        // assert mocked sections repo function called once
        mockSectionsRepo.Verify(d => d.AddNewRecipeSectionsAsync(It.IsAny<List<SectionDTO>>(), recipeId), Times.AtMostOnce);
        // assert mocked ingredient list repo function called once
        mockIngListRepo.Verify(d => d.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections), Times.AtMostOnce);
        // assert mocked instructions repo function called once
        mockInstructionsRepo.Verify(d => d.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections), Times.AtMostOnce);
        // assert mocked tags repo function called once
        mockTagsRepo.Verify(d => d.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId), Times.AtMostOnce);
    }



    // test that AddRecipeManually returns the operation result successfully
    [Fact]
    public async Task AddRecipeManually_Success_NoSections()
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipe;
        // initialize the recipe ID
        int recipeId = 4;
        // initialize sections (no sections)
        List<SectionDTO> sections = [];

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipeAsync(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections))
                    .Returns(1);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections))
                    .Returns(1);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(1);

        // call AddRecipeManually from mocked AddRecipeService
        var result = await addRecipeService.AddRecipeManuallyAsync(newRecipeContent);

        // assert result.success is true
        Assert.True(result.Success);
    }



    [Fact]
    public async Task AddRecipeManually_Success_Sections()
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipeSections;
        // initialize the recipe ID
        int recipeId = 4;
        // initialize the list of ingredient DTOs to add to the table
        var addedSections = newRecipeContent["recipeSections"]?.ToObject<List<SectionDTO>>();
        // initialize return
        (int, List<SectionDTO>) sectionReturn = (3, addedSections!);

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipeAsync(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockSectionsRepo.Setup(r => r.AddNewRecipeSectionsAsync(It.IsAny<List<SectionDTO>>(), recipeId))
                    .ReturnsAsync(sectionReturn);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, addedSections))
                    .Returns(1);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, addedSections))
                    .Returns(1);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(1);

        // call AddRecipeManually from mocked AddRecipeService
        var result = await addRecipeService.AddRecipeManuallyAsync(newRecipeContent);

        // assert result.success is true
        Assert.True(result.Success);
    }



    // test that AddRecipeManually returns an unsuccessful operation if the ingredients, instructions, or tags records added = 0
    [Theory]
    [InlineData(new int[] { 0, 1, 1 })]
    [InlineData(new int[] { 1, 0, 1 })]
    [InlineData(new int[] { 1, 1, 0 })]
    public async Task AddRecipeManually_Failure_NoSections(int[] returnValues)
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipe;
        // initialize the recipe ID
        int recipeId = 4;
        // initialize sections (no sections)
        List<SectionDTO> sections = [];

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipeAsync(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, sections))
                    .Returns(returnValues[0]);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, sections))
                    .Returns(returnValues[1]);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(returnValues[2]);

        // call AddRecipeManually from mocked AddRecipeService
        var result = await addRecipeService.AddRecipeManuallyAsync(newRecipeContent);

        // assert result.success is false
        Assert.False(result.Success);
    }


    [Fact]
    public async Task AddRecipeManually_Failure_Sections()
    {
        // initialize newRecipeContent
        var newRecipeContent = _expectedAddRecipeSections;
        // initialize the recipe ID
        int recipeId = 3;
        // initialize sections
        var addedSections = newRecipeContent["recipeSections"]?.ToObject<List<SectionDTO>>();
        (int, List<SectionDTO>) sectionReturn = (3, addedSections!);

        // set up mock repository functions
        mockRecipeRepo.Setup(r => r.AddNewRecipeAsync(It.IsAny<RecipeDTO>()))
                    .ReturnsAsync(recipeId);
        mockSectionsRepo.Setup(r => r.AddNewRecipeSectionsAsync(It.IsAny<List<SectionDTO>>(), recipeId))
                    .ReturnsAsync(sectionReturn);
        mockIngListRepo.Setup(r => r.AddNewRecipeIng(It.IsAny<List<IngredientListDTO>>(), recipeId, addedSections))
                    .Returns(1);
        mockInstructionsRepo.Setup(r => r.AddNewRecipeIns(It.IsAny<List<InstructionDTO>>(), recipeId, addedSections))
                    .Returns(1);
        mockTagsRepo.Setup(r => r.AddNewRecipeTags(It.IsAny<RecipeTagDTO>(), recipeId))
                    .Returns(1);

        // call AddRecipeManually from mocked AddRecipeService
        var result = await addRecipeService.AddRecipeManuallyAsync(newRecipeContent);

        // assert result.success is false
        Assert.False(result.Success);
    }
}