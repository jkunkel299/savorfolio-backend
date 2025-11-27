using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IIngredientRepository
{
    Task<List<IngredientVariantDTO>> SearchByNameAsync(string searchTerm);
    Task<List<string>> IngredientSearchReturnStringAsync(string? searchTerm);
}
