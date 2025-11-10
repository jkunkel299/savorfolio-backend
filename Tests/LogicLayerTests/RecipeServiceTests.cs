using Moq;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;

namespace Tests.LogicLayerTests;

public class RecipeServiceTests()
{
    // test for business logic function without filter body
    [Fact]
    public async Task RecipeServiceTestEmpty()
    {
        // initialize empty filter
        var request = new RecipeFilterRequestDTO();

        // mock recipe repository interface
        var mockDependency = new Mock<IRecipeRepository>();
        // mock RecipeService
        var recipeService = new RecipeService(mockDependency.Object);

        // call SearchRecipesAsync from mocked RecipeService
        _ = await recipeService.SearchRecipesAsync(request);

        // assert mocked function called once
        mockDependency.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }

    // test for business logic function with filter to include ingredients
    [Fact]
    public async Task RecipeServiceIncludeIngredients()
    {
        // initialize filter to include ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            IncludeIngredients = [143, 2]
        };

        // mock recipe repository interface
        var mockDependency = new Mock<IRecipeRepository>();
        // mock RecipeService
        var recipeService = new RecipeService(mockDependency.Object);

        // call SearchRecipesAsync from mocked RecipeService
        _ = await recipeService.SearchRecipesAsync(request);

        // assert mocked function called once
        mockDependency.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
    
    // test for business logic function with filter to exclude ingredients
    [Fact]
    public async Task RecipeServiceExcludeIngredients()
    {
        // initialize filter to exclude ingredients in test case
        var request = new RecipeFilterRequestDTO
        {
            ExcludeIngredients = [143,2]
        };

        // mock recipe repository interface
        var mockDependency = new Mock<IRecipeRepository>();
        // mock RecipeService
        var recipeService = new RecipeService(mockDependency.Object);

        // call SearchRecipesAsync from mocked RecipeService
        _ = await recipeService.SearchRecipesAsync(request);

        // assert mocked function called once
        mockDependency.Verify(d => d.ReturnRecipesFiltered(request), Times.AtMostOnce);
    }
}