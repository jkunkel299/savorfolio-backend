using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class IngredientVariant
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<IngredientList> IngredientLists { get; set; } = [];

    public virtual required IngredientType Type { get; set; }
}
