using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class Note
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RecipeId { get; set; }

    public string Note1 { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
