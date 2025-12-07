using System.Reflection;
using System.Runtime.Serialization;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Models.enums;

namespace Tests.TestData;

public class CategoryFilterData
{
    public static TheoryData<RecipeFilterRequestDTO> CategoryFilterTestCases() =>
        new()
        {
            {
                new RecipeFilterRequestDTO
                {
                    Recipe_type = DupEnumExtensions.ParseEnumMember<RecipeTypeTag>("Main"),
                }
            },
            {
                new RecipeFilterRequestDTO
                {
                    Meal = DupEnumExtensions.ParseEnumMember<MealTag>("Dinner"),
                }
            },
            {
                new RecipeFilterRequestDTO
                {
                    Cuisine = DupEnumExtensions.ParseEnumMember<CuisineTag>("American"),
                }
            },
            { new RecipeFilterRequestDTO { Dietary = ["Soy-Free"] } },
        };
}

public class DupEnumExtensions()
{
    public static TEnum? ParseEnumMember<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result))
            return result;

        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<EnumMemberAttribute>();
            if (
                attr != null
                && string.Equals(attr.Value, value, StringComparison.OrdinalIgnoreCase)
            )
                return (TEnum)field.GetValue(null)!;
        }

        return null;
    }
}
