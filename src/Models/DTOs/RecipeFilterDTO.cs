namespace savorfolio_backend.Models.DTOs;

public class RecipeFilterRequestDTO
{
    public List<int>? IncludeIngredients { get; set; }
    public List<int>? ExcludeIngredients { get; set; }
    public string? Cuisine { get; set; }
    public List<string>? Dietary { get; set; }
    // more later...
}