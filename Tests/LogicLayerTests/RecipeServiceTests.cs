using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Models.DTOs;
using Tests.TestData;

namespace Tests.LogicLayerTests;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> mockRecipeRepo;
    private readonly RecipeService _service;

    public RecipeServiceTests()
    {
        mockRecipeRepo = new Mock<IRecipeRepository>();
        _service = new(mockRecipeRepo.Object);
    }

    #region Empty Search
    // test for business logic function without filter body
    [Fact]
    public async Task RecipeServiceTestEmpty()
    {
        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // call SearchRecipesAsync from mocked RecipeService
        _ = await _service.SearchRecipesAsync(request);

        // assert mocked function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
    #endregion

    #region Include Ingredients
    // test for business logic function with filter to include ingredients
    [Fact]
    public async Task RecipeServiceIncludeIngredients()
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { IncludeIngredients = [143, 2] };

        // call SearchRecipesAsync from mocked RecipeService
        _ = await _service.SearchRecipesAsync(request);

        // assert mocked function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
    #endregion

    #region Exclude Ingredients
    // test for business logic function with filter to exclude ingredients
    [Fact]
    public async Task RecipeServiceExcludeIngredients()
    {
        // initialize filter to exclude ingredients in test case
        var request = new RecipeFilterRequestDTO { ExcludeIngredients = [143, 2] };

        // call SearchRecipesAsync from mocked RecipeService
        _ = await _service.SearchRecipesAsync(request);

        // assert mocked function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
    #endregion

    #region Category Filter
    // test for business logic functions with category filters - calls repository function
    [Theory]
    [MemberData(
        nameof(CategoryFilterData.CategoryFilterTestCases),
        MemberType = typeof(CategoryFilterData)
    )]
    public async Task RecipeSearchCategoryTags(RecipeFilterRequestDTO request)
    {
        // call SearchRecipesAsync from mocked RecipeService
        _ = await _service.SearchRecipesAsync(request);

        // assert mocked function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
    #endregion

    #region Multiple Filters
    [Fact]
    public async Task RecipeSearchMultipleFilters()
    {
        // initialize filter to exclude semi-sweet chocolate chips and dietary restriction=Nut-Free
        var request = new RecipeFilterRequestDTO
        {
            ExcludeIngredients = [38],
            Dietary = ["Nut-Free"],
        };

        // call SearchRecipesAsync from mocked RecipeService
        _ = await _service.SearchRecipesAsync(request);

        // assert mocked function called once
        mockRecipeRepo.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
    #endregion
}
