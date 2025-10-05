using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IUnitsRepository
{
    Task<List<UnitDTO>> SearchUnitAsync(string searchTerm);
}