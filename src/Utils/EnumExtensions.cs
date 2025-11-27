using System.Reflection;
using System.Runtime.Serialization;

namespace savorfolio_backend.Utils;

public static class EnumExtensions
{
    public static string GetEnumMemberValue<TEnum>(this TEnum enumValue)
        where TEnum : Enum
    {
        var member = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
        if (member == null)
            return enumValue.ToString();

        var enumMemberAttr = member.GetCustomAttribute<EnumMemberAttribute>();
        return enumMemberAttr?.Value ?? enumValue.ToString();
    }

    public static List<string> GetEnumList<TEnum>()
        where TEnum : Enum
    {
        return
        [
            .. typeof(TEnum)
                .GetFields()
                .Where(f => f.IsLiteral)
                .Select(f =>
                    f.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                        .Cast<EnumMemberAttribute>()
                        .FirstOrDefault()
                        ?.Value
                    ?? f.Name
                ),
        ];
    }

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
