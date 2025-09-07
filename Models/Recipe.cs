using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class Recipe
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Servings { get; set; }

    public string? CookTime { get; set; }

    public string? PrepTime { get; set; }

    public int? BakeTemp { get; set; }

    public TempUnitsTag Temp_unit { get; set; }

    public virtual ICollection<IngredientList> IngredientLists { get; set; } = new List<IngredientList>();

    public virtual Instruction? Instruction { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual ICollection<RecipeSection> RecipeSections { get; set; } = new List<RecipeSection>();
}
