using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;

namespace Tests.LogicLayerTests;

public class ViewRecipeServiceTests
{
    private readonly JObject _expectedViewRecipe;
    private readonly JObject _expectedViewRecipeSections;

    // mock recipe repository interface
    private readonly Mock<IRecipeRepository> mockRecipeRepo;

    // mock ingredient list repository interface
    private readonly Mock<IIngListRepository> mockIngListRepo;

    // mock instructions repository interface
    private readonly Mock<IInstructionsRepository> mockInstructionsRepo;

    // mock tags repository interface
    private readonly Mock<ITagsRepository> mockTagsRepo;

    // mock sections repository interface
    private readonly Mock<ISectionsRepository> mockSectionsRepo;

    // mock ViewRecipeService
    private readonly ViewRecipeService viewRecipeService;

    public ViewRecipeServiceTests()
    {
        string viewRecipeFilePath = TestFileHelper.GetProjectPath(
            "ExpectedData/ViewRecipeDTO.json"
        );
        string viewRecipeSectionsFilePath = TestFileHelper.GetProjectPath(
            "ExpectedData/ViewRecipeSectionsDTO.json"
        );
        _expectedViewRecipe = JObject.Parse(File.ReadAllText(viewRecipeFilePath));
        _expectedViewRecipeSections = JObject.Parse(File.ReadAllText(viewRecipeSectionsFilePath));

        // mock recipe repository interface
        mockRecipeRepo = new();
        // mock ingredient list repository interface
        mockIngListRepo = new();
        // mock instructions repository interface
        mockInstructionsRepo = new();
        // mock tags repository interface
        mockTagsRepo = new();
        // mock sections repository interface
        mockSectionsRepo = new();

        viewRecipeService = new(
            mockRecipeRepo.Object,
            mockSectionsRepo.Object,
            mockIngListRepo.Object,
            mockInstructionsRepo.Object,
            mockTagsRepo.Object
        );
    }

    // test that CompileRecipeAsync calls its dependent functions
    [Fact]
    public async Task CompileRecipeAsync_CallsFunctions()
    {
        // initialize test recipe ID
        int recipeId = 2;

        mockRecipeRepo
            .Setup(r => r.ReturnRecipeByIdAsync(recipeId))
            .ReturnsAsync(It.IsAny<RecipeDTO>());
        mockSectionsRepo
            .Setup(r => r.GetSectionsByRecipeAsync(recipeId))
            .ReturnsAsync(It.IsAny<List<SectionDTO>>());
        mockIngListRepo
            .Setup(r => r.GetIngredientsByRecipeAsync(recipeId))
            .ReturnsAsync(It.IsAny<List<IngredientListDTO>>());
        mockInstructionsRepo
            .Setup(r => r.GetInstructionsByRecipeAsync(recipeId))
            .ReturnsAsync(It.IsAny<List<InstructionDTO>>());
        mockTagsRepo.Setup(r => r.GetTagsByRecipe(recipeId)).Returns(It.IsAny<TagStringsDTO>());

        // call CompileRecipeAsync from mocked ViewRecipeService
        _ = await viewRecipeService.CompileRecipeAsync(recipeId);

        // assert mocked recipe repo function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipeByIdAsync(recipeId), Times.AtMostOnce);
        // assert mocked ingredient list repo function called once
        mockIngListRepo.Verify(d => d.GetIngredientsByRecipeAsync(recipeId), Times.AtMostOnce);
        // assert mocked instructions repo function called once
        mockInstructionsRepo.Verify(
            d => d.GetInstructionsByRecipeAsync(recipeId),
            Times.AtMostOnce
        );
        // assert mocked tags repo function called once
        mockTagsRepo.Verify(d => d.GetTagsByRecipe(recipeId), Times.AtMostOnce);
    }

    [Fact]
    public async Task CompileRecipesAsync_CreatesDTO_NoSections()
    {
        // initialize test recipe ID
        int recipeId = 2;

        // set up expected DTO return from recipe repository
        var mockRecipeSummary =
            _expectedViewRecipe["RecipeSummary"]?.ToObject<RecipeDTO>() ?? new RecipeDTO();
        mockRecipeRepo
            .Setup(d => d.ReturnRecipeByIdAsync(recipeId))
            .ReturnsAsync(mockRecipeSummary);
        // set up expected DTO return from sections repository (no sections)
        var mockSections =
            _expectedViewRecipe["RecipeSections"]?.ToObject<List<SectionDTO>>() ?? [];
        // set up expected DTO return from ingredient list repository
        var mockIngList =
            _expectedViewRecipe["Ingredients"]?.ToObject<List<IngredientListDTO>>() ?? [];
        mockIngListRepo
            .Setup(d => d.GetIngredientsByRecipeAsync(recipeId))
            .ReturnsAsync(mockIngList);
        // set up expected DTO return from instructions repository
        var mockInsList =
            _expectedViewRecipe["Instructions"]?.ToObject<List<InstructionDTO>>() ?? [];
        mockInstructionsRepo
            .Setup(d => d.GetInstructionsByRecipeAsync(recipeId))
            .ReturnsAsync(mockInsList);
        // set up expected DTO return from tags repository
        var mockTags =
            _expectedViewRecipe["RecipeTags"]?.ToObject<TagStringsDTO>() ?? new TagStringsDTO();
        mockTagsRepo.Setup(d => d.GetTagsByRecipe(recipeId)).Returns(mockTags);

        // initialize expected FullRecipeDTO return
        FullRecipeDTO expectedReturn = new()
        {
            RecipeId = recipeId,
            RecipeSummary = mockRecipeSummary,
            RecipeTags = mockTags,
            RecipeSections = mockSections,
            Ingredients = mockIngList,
            Instructions = mockInsList,
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

    [Fact]
    public async Task CompileRecipesAsync_CreatesDTO_Sections()
    {
        // initialize test recipe ID
        int recipeId = 3;

        // set up expected DTO return from recipe repository
        var mockRecipeSummary =
            _expectedViewRecipeSections["RecipeSummary"]?.ToObject<RecipeDTO>() ?? new RecipeDTO();
        mockRecipeRepo
            .Setup(d => d.ReturnRecipeByIdAsync(recipeId))
            .ReturnsAsync(mockRecipeSummary);
        // set up expected DTO return from sections repository
        var mockSections =
            _expectedViewRecipeSections["RecipeSections"]?.ToObject<List<SectionDTO>>() ?? [];
        mockSectionsRepo
            .Setup(d => d.GetSectionsByRecipeAsync(recipeId))
            .ReturnsAsync(mockSections);
        // set up expected DTO return from ingredient list repository
        var mockIngList =
            _expectedViewRecipeSections["Ingredients"]?.ToObject<List<IngredientListDTO>>() ?? [];
        mockIngListRepo
            .Setup(d => d.GetIngredientsByRecipeAsync(recipeId))
            .ReturnsAsync(mockIngList);
        // set up expected DTO return from instructions repository
        var mockInsList =
            _expectedViewRecipeSections["Instructions"]?.ToObject<List<InstructionDTO>>() ?? [];
        mockInstructionsRepo
            .Setup(d => d.GetInstructionsByRecipeAsync(recipeId))
            .ReturnsAsync(mockInsList);
        // set up expected DTO return from tags repository
        var mockTags =
            _expectedViewRecipeSections["RecipeTags"]?.ToObject<TagStringsDTO>()
            ?? new TagStringsDTO();
        mockTagsRepo.Setup(d => d.GetTagsByRecipe(recipeId)).Returns(mockTags);

        // initialize expected FullRecipeDTO return
        FullRecipeDTO expectedReturn = new()
        {
            RecipeId = recipeId,
            RecipeSummary = mockRecipeSummary,
            RecipeTags = mockTags,
            RecipeSections = mockSections,
            Ingredients = mockIngList,
            Instructions = mockInsList,
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
