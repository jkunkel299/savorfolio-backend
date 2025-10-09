using savorfolio_backend.Models.enums;

namespace savorfolio_backend.Models.DTOs;

public class TagStringsDTO
{
    public int RecipeId { get; set; }

    public string? Meal { get; set; }

    public string? Recipe_type { get; set; }

    public string? Cuisine { get; set; }

    public List<string> Dietary { get; set; } = [];

}