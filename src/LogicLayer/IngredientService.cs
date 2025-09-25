using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Interfaces;

namespace savorfolio_backend.LogicLayer;

public class IngredientService(IIngredientRepository ingredientRepository) : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;

    public async Task<List<IngredientVariantDTO>> SearchIngredientsAsync(string searchTerm)
    {
        // Call the repository method (Data Access Layer)
        var ingredients = await _ingredientRepository.SearchByNameAsync(searchTerm);

        return ingredients;
    }
}