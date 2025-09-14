using savorfolio_backend.DataAccess;
using System.Text.Json;

namespace savorfolio_backend.LogicLayer;

public class IngredientService(IngredientRepository ingredientRepository)
{
    private readonly IngredientRepository _ingredientRepository = ingredientRepository;

    public async Task<string> SearchIngredientsAsync(string searchTerm)
    {
        // Call the repository method (Data Access Layer)
        var ingredients = await _ingredientRepository.SearchByNameAsync(searchTerm);
        
        // Convert to JSON
        string ingredientsJson = JsonSerializer.Serialize(ingredients);

        return ingredientsJson;
    }
}