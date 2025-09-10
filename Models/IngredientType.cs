using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class IngredientType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<IngredientVariant> IngredientVariants { get; set; } = [];
}
