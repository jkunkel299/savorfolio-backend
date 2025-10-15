using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace savorfolio_backend.LogicLayer;

public static class JsonParseService
{
    public static async Task<JObject> ParseJson(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        var requestBody = await reader.ReadToEndAsync();

        JObject newJson;
        try
        {
            newJson = JObject.Parse(requestBody);
        }
        catch (JsonReaderException ex)
        {
            newJson = JObject.Parse($"Invalid JSON format: {ex.Message}");
            // return Results.BadRequest($"Invalid JSON format: {ex.Message}");
        }
        return newJson;
    }
}

