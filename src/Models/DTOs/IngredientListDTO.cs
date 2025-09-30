namespace savorfolio_backend.Models.DTOs;

public class IngredientListDTO
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    // public int? SectionId { get; set; }

    public int IngredientOrder { get; set; }

    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public string? Quantity { get; set; }

    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;

    public string? Qualifier { get; set; } = "";
}
