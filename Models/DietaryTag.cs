using System.ComponentModel;

namespace savorfolio_backend.Models;

public enum DietaryTag
{
    Vegan,
    Vegetarian,
    [Description("Gluten-Free")] GlutenFree,
    [Description("Dairy-Free")] DairyFree,
    [Description("Nut-Free")] NutFree,
    [Description("Soy-Free")] SoyFree,
    [Description("Egg-Free")] EggFree,
    Halal,
    Kosher,
    Keto,
    Paleo,
    [Description("Low-Sodium")] LowSodium,
    [Description("Low-Sugar")] LowSugar,
    Whole30
}