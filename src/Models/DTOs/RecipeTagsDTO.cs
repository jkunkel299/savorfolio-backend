using savorfolio_backend.Models.enums;

namespace savorfolio_backend.Models.DTOs;

public class RecipeTagDTO
{
    public int RecipeId { get; set; }

    public MealTag? Meal { get; set; }

    public RecipeTypeTag? Recipe_type { get; set; }

    public CuisineTag? Cuisine { get; set; }

    public List<DietaryTag> Dietary { get; set; } = [];

}