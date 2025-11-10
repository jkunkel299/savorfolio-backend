namespace savorfolio_backend.Models;

public partial class IngredientVariant
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? PluralName { get; set; }

    public virtual ICollection<IngredientList> IngredientLists { get; set; } = [];

    public virtual IngredientType? Type { get; set; } = null;
}
