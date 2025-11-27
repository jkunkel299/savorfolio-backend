namespace savorfolio_backend.Models.DTOs;

public partial class SectionDTO
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public string SectionName { get; set; } = null!;

    public int SortOrder { get; set; }
}
