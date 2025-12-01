using System.Text.Json;

namespace Tests.Helpers;

public class JsonToList
{
    private static JsonSerializerOptions JsonOptions =>
        new() { PropertyNameCaseInsensitive = true };

    // Generic list from JSON
    public static List<T> JsonFileToList<T>(string filePath)
        where T : class
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed file not found: {filePath}");

        var json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        var items = JsonSerializer.Deserialize<List<T>>(json, JsonOptions);
        if (items == null || items.Count == 0)
            return [];

        return items;
    }
}
