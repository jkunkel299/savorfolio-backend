
namespace savorfolio_backend.Models.DTOs;

public class RecipeDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Servings { get; set; }

    public string? CookTime { get; set; }

    public string? PrepTime { get; set; }

    public int? BakeTemp { get; set; }

    public string? Temp_unit { get; set; }

    public string? Description { get; set; }

    public List<IngredientListDTO> Ingredients { get; set; } = [];
}