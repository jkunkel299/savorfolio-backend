namespace savorfolio_backend.Models.DTOs;

public partial class IngredientVariantDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public required string IngredientCategory { get; set; }
}
