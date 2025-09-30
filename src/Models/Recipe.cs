namespace savorfolio_backend.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Servings { get; set; }

    public string? CookTime { get; set; }

    public string? PrepTime { get; set; }

    public int? BakeTemp { get; set; }

    public string? Temp_unit { get; set; } // changed - see if this breaks

    public virtual ICollection<IngredientList> IngredientLists { get; set; } = [];

    public virtual ICollection<Instruction> Instructions { get; set; } = [];

    public virtual ICollection<Note> Notes { get; set; } = [];

    public virtual ICollection<RecipeSection> RecipeSections { get; set; } = [];
}
