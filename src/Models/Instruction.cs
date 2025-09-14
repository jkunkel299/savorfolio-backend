using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class Instruction
{
    public int Id { get; set; }

    public int? RecipeId { get; set; }

    public int? SectionId { get; set; }

    public int StepNumber { get; set; }

    public string InstructionText { get; set; } = null!;

    public virtual Recipe? Recipe { get; set; }

    public virtual RecipeSection? Section { get; set; }
}
