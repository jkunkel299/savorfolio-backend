using savorfolio_backend.Data;
using savorfolio_backend.Models;
using System.Text.Json;

namespace Tests.Helpers;

public class InMemoryDbSeeder
{
    private static JsonSerializerOptions JsonOptions => new() { PropertyNameCaseInsensitive = true };

    // Generic seeder for simple entity lists
    public static void SeedFromJson<T>(AppDbContext context, string filePath) where T : class
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed file not found: {filePath}");

        var json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var items = JsonSerializer.Deserialize<List<T>>(json, JsonOptions);
        if (items == null || items.Count == 0)
            return;

        // Remove existing rows for T
        var dbSet = context.Set<T>();
        var existing = dbSet.AsQueryable().ToList();
        if (existing.Count != 0)
        {
            dbSet.RemoveRange(existing);
            context.SaveChanges();
        }

        dbSet.AddRange(items);
        context.SaveChanges();
    }

    // Specialized variant seeder to ensure TypeId matches seeded IngredientType rows
    public static void SeedVariantsFromJson(AppDbContext context, string variantFilePath)
    {
        if (!File.Exists(variantFilePath))
            throw new FileNotFoundException($"Seed file not found: {variantFilePath}");

        var json = File.ReadAllText(variantFilePath);
        if (string.IsNullOrWhiteSpace(json)) return;

        var variants = JsonSerializer.Deserialize<List<IngredientVariant>>(json, JsonOptions) ?? new List<IngredientVariant>();

        // Clear existing variants
        var existing = context.IngredientVariants.AsQueryable().ToList();
        if (existing.Count != 0)
        {
            context.IngredientVariants.RemoveRange(existing);
            context.SaveChanges();
        }

        // If JSON included nested Type objects (or type names), resolve them to TypeId
        foreach (var v in variants)
        {
            if (v.Type != null)
            {
                // attempt to find by name first
                var matchedType = context.IngredientTypes.FirstOrDefault(t => t.Name == v.Type.Name);
                if (matchedType != null)
                {
                    v.TypeId = matchedType.Id;
                }
                // drop the navigation object to avoid EF trying to insert duplicates
                v.Type = null;
            }
        }

        context.IngredientVariants.AddRange(variants);
        context.SaveChanges();
    }
}