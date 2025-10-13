using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class UnitsService(IUnitsRepository unitsRepository) : IUnitsService
{
    private readonly IUnitsRepository _unitsRepository = unitsRepository;

    public async Task<List<UnitDTO>> SearchUnitsAsync(string searchTerm)
    {
        // Call the repository method (Data Access Layer)
        var units = await _unitsRepository.SearchUnitTableAsync(searchTerm);

        return units;
    }
}