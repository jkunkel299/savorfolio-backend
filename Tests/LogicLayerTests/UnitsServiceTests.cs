using Moq;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Interfaces;

namespace Tests.LogicLayerTests;

public class UnitsServiceTests()
{
    [Fact]
    public async Task UnitsServiceCallsDependency()
    {
        // initialize search term
        string searchTerm = "te";

        // mock units repository interface
        var mockUnitsRepo = new Mock<IUnitsRepository>();
        // mock UnitsService
        var unitsService = new UnitsService(mockUnitsRepo.Object);

        // call SearchUnitsAsync from mocked UnitsService
        _ = await unitsService.SearchUnitsAsync(searchTerm);

        // assert mocked function called once
        mockUnitsRepo.Verify(d => d.SearchUnitTableAsync(searchTerm), Times.Once);
    }
}