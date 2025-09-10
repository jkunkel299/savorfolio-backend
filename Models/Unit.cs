using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class Unit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Abbreviation { get; set; }

    public virtual ICollection<IngredientList> IngredientLists { get; set; } = [];
}
