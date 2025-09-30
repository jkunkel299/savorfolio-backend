using System.Reflection;
using System.Runtime.Serialization;

namespace savorfolio_backend.Utils;

public static class EnumExtensions
{
    public static string GetEnumMemberValue<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        var member = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
        if (member == null) return enumValue.ToString();

        var enumMemberAttr = member.GetCustomAttribute<EnumMemberAttribute>();
        return enumMemberAttr?.Value ?? enumValue.ToString();
    }
}