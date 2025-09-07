using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class UserRecipe
{
    public int UserId { get; set; }

    public int? RecipeId { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User User { get; set; } = null!;
}
