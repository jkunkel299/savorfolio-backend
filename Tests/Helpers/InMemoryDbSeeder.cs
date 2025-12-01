using System.Text.Json;
using System.Text.Json.Serialization;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace Tests.Helpers;

public class InMemoryDbSeeder
{
    private static JsonSerializerOptions JsonOptions =>
        new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false),
            },
        };

    // Generic seeder for simple entity lists
    public static void SeedFromJson<T>(AppDbContext context, string filePath)
        where T : class
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
        if (string.IsNullOrWhiteSpace(json))
            return;

        var variants = JsonSerializer.Deserialize<List<IngredientVariant>>(json, JsonOptions) ?? [];

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
                var matchedType = context.IngredientTypes.FirstOrDefault(t =>
                    t.Name == v.Type.Name
                );
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

    // specialized variant seeder for recipe sections
    public static void SeedSectionsFromJson(AppDbContext context, string sectionFilePath)
    {
        if (!File.Exists(sectionFilePath))
            throw new FileNotFoundException($"Seed file not found: {sectionFilePath}");

        var json = File.ReadAllText(sectionFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var sectionLists = JsonSerializer.Deserialize<List<RecipeSection>>(json, JsonOptions) ?? [];

        // Clear existing intruction list items
        var existing = context.RecipeSections.AsQueryable().ToList();
        if (existing.Count != 0)
        {
            context.RecipeSections.RemoveRange(existing);
            context.SaveChanges();
        }

        // If JSON included nested Recipe objects, resolve them to RecipeId; resolve sections to SectionId
        foreach (var i in sectionLists)
        {
            if (i.Recipe != null)
            {
                // attempt to find by name
                var matchedRecipe = context.Recipes.FirstOrDefault(r => r.Name == i.Recipe.Name);
                if (matchedRecipe != null)
                {
                    i.RecipeId = matchedRecipe.Id;
                }
            }
        }

        context.RecipeSections.AddRange(sectionLists);
        context.SaveChanges();
    }

    // specialized variant seeder for ingredient lists
    public static void SeedIngredientListsFromJson(AppDbContext context, string ingListFilePath)
    {
        if (!File.Exists(ingListFilePath))
            throw new FileNotFoundException($"Seed file not found: {ingListFilePath}");

        var json = File.ReadAllText(ingListFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var ingredientLists =
            JsonSerializer.Deserialize<List<IngredientList>>(json, JsonOptions) ?? [];

        // Clear existing ingredient list items
        var existing = context.IngredientLists.AsQueryable().ToList();
        if (existing.Count != 0)
        {
            context.IngredientLists.RemoveRange(existing);
            context.SaveChanges();
        }

        // If JSON included nested Recipe objects, resolve them to RecipeId; resolve to SectionId
        foreach (var i in ingredientLists)
        {
            if (i.Recipe != null)
            {
                // attempt to find by name first
                var matchedRecipe = context.Recipes.FirstOrDefault(r => r.Name == i.Recipe.Name);
                if (matchedRecipe != null)
                {
                    i.RecipeId = matchedRecipe.Id;
                }
            }
            // if (i.SectionId != null)
            // {
            //     // attempt to find by name
            //     var matchedSection = context.RecipeSections.FirstOrDefault(r => r.SectionName == i.Section!.SectionName);
            //     if (matchedSection != null)
            //     {
            //         i.SectionId = matchedSection.Id;
            //     }
            // }
        }

        context.IngredientLists.AddRange(ingredientLists);
        context.SaveChanges();
    }

    // specialized variant seeder for instruction lists
    public static void SeedInstructionsFromJson(AppDbContext context, string insListFilePath)
    {
        if (!File.Exists(insListFilePath))
            throw new FileNotFoundException($"Seed file not found: {insListFilePath}");

        var json = File.ReadAllText(insListFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var intructionLists =
            JsonSerializer.Deserialize<List<Instruction>>(json, JsonOptions) ?? [];

        // Clear existing intruction list items
        var existing = context.Instructions.AsQueryable().ToList();
        if (existing.Count != 0)
        {
            context.Instructions.RemoveRange(existing);
            context.SaveChanges();
        }

        // If JSON included nested Recipe objects, resolve them to RecipeId; resolve sections to SectionId
        foreach (var i in intructionLists)
        {
            if (i.Recipe != null)
            {
                // attempt to find by name
                var matchedRecipe = context.Recipes.FirstOrDefault(r => r.Name == i.Recipe.Name);
                if (matchedRecipe != null)
                {
                    i.RecipeId = matchedRecipe.Id;
                }
            }
            // if (i.SectionId != null)
            // {
            //     // attempt to find by name
            //     var matchedSection = context.RecipeSections.FirstOrDefault(r => r.SectionName == i.Section!.SectionName);
            //     if (matchedSection != null)
            //     {
            //         i.SectionId = matchedSection.Id;
            //     }
            // }
        }

        context.Instructions.AddRange(intructionLists);
        context.SaveChanges();
    }

    public static void SeedTagsFromJson(AppDbContext context, string tagsFilePath)
    {
        if (!File.Exists(tagsFilePath))
            throw new FileNotFoundException($"Seed file not found: {tagsFilePath}");

        var json = File.ReadAllText(tagsFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var items = JsonSerializer.Deserialize<List<RecipeTag>>(json, JsonOptions);
        if (items == null || items.Count == 0)
            return;

        // Convert DietaryTags strings into EnumMember-mapped enum names (optional normalization)
        foreach (var item in items)
        {
            if (item.Dietary != null && item.Dietary.Length > 0)
            {
                // Normalize tags: remove whitespace and ensure consistent EnumMember formatting
                item.Dietary = [.. item.Dietary.Select(t => t.Trim())];
            }
        }

        // Remove existing rows
        var existing = context.RecipeTags.AsQueryable().ToList();
        if (existing.Count != 0)
        {
            context.RecipeTags.RemoveRange(existing);
            context.SaveChanges();
        }

        context.RecipeTags.AddRange(items);
        context.SaveChanges();
    }
}
