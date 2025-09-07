using System;
using System.Collections.Generic;

namespace savorfolio_backend.Models;

public partial class RecipeTag
{
    public int? RecipeId { get; set; }

    public int? CustomTagId { get; set; }

    public int? NoteId { get; set; }

    public MealTag Meal { get; set; }

    public RecipeTypeTag Recipe_type { get; set; }

    public CuisineTag Cuisine { get; set; }

    public DietaryTag Dietary { get; set; }

    public virtual CustomTag? CustomTag { get; set; }

    public virtual Note? Note { get; set; }

    public virtual Recipe? Recipe { get; set; }
}
