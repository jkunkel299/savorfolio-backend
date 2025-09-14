using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class RecipeSection
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public string SectionName { get; set; } = null!;

    public int SortOrder { get; set; }

    public virtual ICollection<Instruction> Instructions { get; set; } = [];

    public virtual Recipe Recipe { get; set; } = null!;
}
