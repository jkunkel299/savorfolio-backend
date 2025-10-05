using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace savorfolio_backend.Models.enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum CuisineTag
{
    Italian,
    Mexican,
    Chinese,
    Japanese,
    Indian,
    Thai,
    French,
    Spanish,
    Greek,
    Korean,
    Vietnamese,
    Lebanese,
    Moroccan,
    Ethiopian,
    Brazilian,
    Argentinian,
    Caribbean,
    American,
    [EnumMember(Value = "Middle Eastern")] MiddleEastern,
    Turkish,
    Hungarian,
    Bavarian
}