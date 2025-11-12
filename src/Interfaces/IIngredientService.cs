using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IIngredientService
{
    Task<List<IngredientVariantDTO>> SearchIngredientsAsync(string searchTerm);
}
