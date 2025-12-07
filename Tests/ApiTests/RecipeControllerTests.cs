using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;
using Tests.TestData;

namespace Tests.ApiTests;

public class RecipeControllerTests()
{
    #region Empty search
    // test for MapRecipeSearch controller function without filter body
    [Fact]
    public async Task RecipeControllerTestEmpty()
    {
        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }
    #endregion

    #region Include Ingredients
    // test for MapRecipeSearch controller function with filter to include ingredients
    [Fact]
    public async Task RecipeControllerTestIncludeIngredients()
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO { IncludeIngredients = [143, 2] };

        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }
    #endregion

    #region Exclude Ingredients
    // test for MapRecipeSearch controller function with filter to exclude ingredients
    [Fact]
    public async Task RecipeControllerTestExcludeIngredients()
    {
        // initialize filter to exclude ingredients in test case
        var request = new RecipeFilterRequestDTO { ExcludeIngredients = [143, 2] };

        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }
    #endregion

    #region Category Filtering
    // test for MapRecipeSearch controller function with category filtering
    [Theory]
    [MemberData(
        nameof(CategoryFilterData.CategoryFilterTestCases),
        MemberType = typeof(CategoryFilterData)
    )]
    public async Task RecipeSearchCategoryTags(RecipeFilterRequestDTO request)
    {
        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }
    #endregion

    #region Multiple Filters
    // test for MapRecipeSearch controller function with multiple simultaneous filters
    [Fact]
    public async Task RecipeControllerTestMultipleFilters()
    {
        // initialize filter to exclude semi-sweet chocolate chips and dietary restriction=Nut-Free
        var request = new RecipeFilterRequestDTO
        {
            ExcludeIngredients = [38],
            Dietary = ["Nut-Free"],
        };

        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }
    #endregion

    #region Get Recipe By ID
    // test MapRecipeById controller function
    [Fact]
    public async Task RecipeControllerGetRecipeById()
    {
        // initialize test recipe ID
        int recipeId = 2;

        // mock view recipe service interface
        var mockViewRecipeService = new Mock<IViewRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeViewEndpoint(
            recipeId,
            mockViewRecipeService.Object
        );

        // assert mocked function called once
        mockViewRecipeService.Verify(d => d.CompileRecipeAsync(recipeId), Times.Once);
    }
    #endregion
}
