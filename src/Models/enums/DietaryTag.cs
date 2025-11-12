using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace savorfolio_backend.Models.enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum DietaryTag
{
    Vegan,
    Vegetarian,

    [EnumMember(Value = "Gluten-Free")]
    GlutenFree,

    [EnumMember(Value = "Dairy-Free")]
    DairyFree,

    [EnumMember(Value = "Nut-Free")]
    NutFree,

    [EnumMember(Value = "Soy-Free")]
    SoyFree,

    [EnumMember(Value = "Egg-Free")]
    EggFree,
    Halal,
    Kosher,
    Keto,
    Paleo,

    [EnumMember(Value = "Low-Sodium")]
    LowSodium,

    [EnumMember(Value = "Low-Sugar")]
    LowSugar,
    Whole30,
}
