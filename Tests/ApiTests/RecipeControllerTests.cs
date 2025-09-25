using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;

namespace Tests.ApiTests;

public class RecipeControllerTests()
{
    // test for controller function without filter body
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

    // test for controller function with filter to include ingredients
    [Fact]
    public async Task RecipeControllerTestIncludeIngredients()
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [143, 2]
        };

        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }

    // test for controller function with filter to include ingredients
    [Fact]
    public async Task RecipeControllerTestExcludeIngredients()
    {
        // initialize filter to exclude ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            ExcludeIngredients = [143, 2]
        };

        // mock recipe service interface
        var mockDependency = new Mock<IRecipeService>();

        // call endpoint
        _ = await RecipeEndpointsHelper.InvokeRecipeSearchEndpoint(request, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchRecipesAsync(request), Times.Once);
    }
}