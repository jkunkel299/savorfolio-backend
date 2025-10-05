using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IUnitsService
{
    Task<List<UnitDTO>> SearchUnitsAsync(string searchTerm);
}