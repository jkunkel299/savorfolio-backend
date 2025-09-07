using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class IngredientList
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public int? SectionId { get; set; }

    public string Quantity { get; set; } = null!;

    public int UnitId { get; set; }

    public int IngredientId { get; set; }

    public virtual IngredientVariant Ingredient { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;
}
