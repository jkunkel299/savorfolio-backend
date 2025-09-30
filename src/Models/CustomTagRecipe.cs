namespace savorfolio_backend.Models;

public partial class CustomTagRecipe
{
    public int CustomTagId { get; set; }

    public int? RecipeId { get; set; }

    public virtual CustomTag CustomTag { get; set; } = null!;

    public virtual Recipe? Recipe { get; set; }
    
}
