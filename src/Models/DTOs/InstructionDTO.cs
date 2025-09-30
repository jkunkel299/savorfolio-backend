namespace savorfolio_backend.Models.DTOs;

public class InstructionDTO
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    // public int? SectionId { get; set; }

    public int StepNumber { get; set; }

    public string InstructionText { get; set; } = null!;

    // public virtual RecipeSection? Section { get; set; }
}