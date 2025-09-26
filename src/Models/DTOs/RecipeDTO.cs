
namespace savorfolio_backend.Models.DTOs;

public class RecipeDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Servings { get; set; }

    public string? CookTime { get; set; }

    public string? PrepTime { get; set; }

    public int? BakeTemp { get; set; }

    public string? Temp_unit { get; set; }

    public List<IngredientListDTO> Ingredients { get; set; } = [];

    public static implicit operator List<object>(RecipeDTO v)
    {
        throw new NotImplementedException();
    }

    // public virtual Instruction? Instruction { get; set; }

    // public virtual ICollection<Note> Notes { get; set; } = [];

    // public virtual ICollection<RecipeSection> RecipeSections { get; set; } = [];
}