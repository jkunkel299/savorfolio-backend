using savorfolio_backend.Interfaces;

namespace savorfolio_backend.Models.DTOs;

public partial class IngredientVariantDTO : IDTOInterface
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PluralName { get; set; } = "";

    public int TypeId { get; set; }

    public required string IngredientCategory { get; set; }
}
