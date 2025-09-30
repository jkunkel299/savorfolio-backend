using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using savorfolio_backend.Models.enums;

namespace savorfolio_backend.Models;

public partial class RecipeTag
{
    public int RecipeId { get; set; }

    public MealTag? Meal { get; set; }

    public RecipeTypeTag? Recipe_type { get; set; }

    public CuisineTag? Cuisine { get; set; }

    // [JsonConverter(typeof(StringEnumConverter))]
    public List<string> Dietary { get; set; } = [];

    public virtual Recipe? Recipe { get; set; }
}
