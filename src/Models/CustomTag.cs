using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class CustomTag
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual User? User { get; set; }
}
