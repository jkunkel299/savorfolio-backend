using Moq;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Interfaces;

namespace Tests.LogicLayerTests;

public class IngredientServiceTests()
{
    [Fact]
    public async Task IngredientServiceTest1()
    {
        // initialize search term
        string searchTerm = "chicken";

        // mock mock ingredient repository interface
        var mockDependency = new Mock<IIngredientRepository>();
        // mock IngredientService
        var ingredientService = new IngredientService(mockDependency.Object);

        // call SearchIngredientsAsync from mocked IngredientService
        _ = await ingredientService.SearchIngredientsAsync(searchTerm);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchByNameAsync(searchTerm), Times.Once);
    }
}