using savorfolio_backend.Models.enums;

namespace savorfolio_backend.Models.DTOs;

public class RecipeFilterRequestDTO
{
    public List<int>? IncludeIngredients { get; set; }
    public List<int>? ExcludeIngredients { get; set; }
    public string? Recipe_typeString { get; set; }
    public RecipeTypeTag? Recipe_type { get; set; }
    public string? MealString { get; set; }
    public MealTag? Meal { get; set; }
    public string? CuisineString { get; set; }
    public CuisineTag? Cuisine { get; set; }
    public List<string>? Dietary { get; set; }
    public int? UserId { get; set; }
    public string? RecipeName { get; set; }
}
