using savorfolio_backend.DataAccess;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class IngredientService(IngredientRepository ingredientRepository)
{
    private readonly IngredientRepository _ingredientRepository = ingredientRepository;

    public async Task<List<IngredientVariantDTO>> SearchIngredientsAsync(string searchTerm)
    {
        // Call the repository method (Data Access Layer)
        var ingredients = await _ingredientRepository.SearchByNameAsync(searchTerm);

        return ingredients;
    }
}