using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using savorfolio_backend.Data;
using Tests.Helpers;
using System.Text.Json;

namespace IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    public AppDbContext Context { get; }

    public DatabaseFixture()
    {
        // Add connection string from environment variables
        Env.Load();
        ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DATABASE_CONNECTION")
            ?? throw new InvalidOperationException("ConnectionStrings__DATABASE_CONNECTION not found in .env");

        // Configure TestDbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        Context = new AppDbContext(options);

        // Reset and seed database when fixture is created 
        ResetDatabase().Wait();
        SeedDatabase().Wait();
    }

    private async Task ResetDatabase()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        // truncate recipes, ingredients, instructions, and tags tables
        using var cmd = new NpgsqlCommand(@"
            TRUNCATE TABLE ""Recipe"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Recipe_Sections"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Ingredient_Lists"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Instructions"" RESTART IDENTITY CASCADE;
            TRUNCATE TABLE ""Recipe_Tags"" RESTART IDENTITY CASCADE;
        ", conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task SeedDatabase()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        // load and insert recipe data
        string recipeFilePath = TestFileHelper.GetProjectPath("SeedData/Recipe.json");
        string sectionsFilePath = TestFileHelper.GetProjectPath("SeedData/RecipeSections.json");
        string ingListFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Lists.json");
        string insListFilePath = TestFileHelper.GetProjectPath("SeedData/Instructions.json");
        string tagsFilePath = TestFileHelper.GetProjectPath("SeedData/RecipeTags.json");

        // read data from files
        string recipesJson = await File.ReadAllTextAsync(recipeFilePath);
        string sectionsJson = await File.ReadAllTextAsync(sectionsFilePath);
        string ingListJson = await File.ReadAllTextAsync(ingListFilePath);
        string instructJson = await File.ReadAllTextAsync(insListFilePath);
        string recipeTagsJson = await File.ReadAllTextAsync(tagsFilePath);

        // convert to seed data objects
        var recipes = JsonSerializer.Deserialize<List<RecipeSeed>>(recipesJson)!;
        var sections = JsonSerializer.Deserialize<List<SectionSeed>>(sectionsJson)!;
        var ingList = JsonSerializer.Deserialize<List<IngredientListSeed>>(ingListJson)!;
        var instructions = JsonSerializer.Deserialize<List<InstructionSeed>>(instructJson)!;
        var recipeTags = JsonSerializer.Deserialize<List<RecipeTagsSeed>>(recipeTagsJson)!;

        // load records into Recipes table
        foreach (var recipe in recipes)
        {
            using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO ""Recipe"" (name, servings, cook_time, prep_time, bake_temp, temp_unit, description) 
                VALUES (@name, @servings, @cook_time, @prep_time, @bake_temp, @temp_unit, @description)",
            conn);
            insertCmd.Parameters.AddWithValue("@name", recipe.Name);
            insertCmd.Parameters.AddWithValue("@servings", recipe.Servings ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@cook_time", recipe.CookTime ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@prep_time", recipe.PrepTime ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@bake_temp", recipe.BakeTemp ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@temp_unit", recipe.Temp_unit ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@description", recipe.Description ?? (object)DBNull.Value);
            await insertCmd.ExecuteNonQueryAsync();
        }

        // load records into Recipe_Sections table
        foreach (var section in sections)
        {
            using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO ""Recipe_Sections"" (recipe_id, section_name, sort_order)
                VALUES (@recipe_id, @section_name, @sort_order)",
            conn);
            insertCmd.Parameters.AddWithValue("@recipe_id", section.RecipeId);
            insertCmd.Parameters.AddWithValue("@section_name", section.SectionName);
            insertCmd.Parameters.AddWithValue("@sort_order", section.SortOrder);
            await insertCmd.ExecuteNonQueryAsync();
        }

        // load records into Ingredient_Lists table
        foreach (var ing in ingList)
        {
            using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO ""Ingredient_Lists"" (recipe_id, section_id, quantity, unit_id, ingredient_id, ingredient_order, qualifier) 
                VALUES (@recipe_id, @section_id, @quantity, @unit_id, @ingredient_id, @ingredient_order, @qualifier)",
            conn);
            // insertCmd.Parameters.AddWithValue("@id", ing.Id);
            insertCmd.Parameters.AddWithValue("@recipe_id", ing.RecipeId);
            insertCmd.Parameters.AddWithValue("@section_id", ing.SectionId ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@quantity", ing.Quantity ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@unit_id", ing.UnitId);
            insertCmd.Parameters.AddWithValue("@ingredient_id", ing.IngredientId);
            insertCmd.Parameters.AddWithValue("@ingredient_order", ing.IngredientOrder);
            insertCmd.Parameters.AddWithValue("@qualifier", ing.Qualifier ?? (object)DBNull.Value);
            await insertCmd.ExecuteNonQueryAsync();
        }

        // load records into Instructions table
        foreach (var ins in instructions)
        {
            using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO ""Instructions"" (recipe_id, section_id, step_number, instruction_text) 
                VALUES (@recipe_id, @section_id, @step, @text)",
            conn);
            // insertCmd.Parameters.AddWithValue("@id", ins.Id);
            insertCmd.Parameters.AddWithValue("@recipe_id", ins.RecipeId);
            insertCmd.Parameters.AddWithValue("@section_id", ins.SectionId ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@step", ins.StepNumber);
            insertCmd.Parameters.AddWithValue("@text", ins.InstructionText);
            await insertCmd.ExecuteNonQueryAsync();
        }

        // load records into Recipe_Tags table
        foreach (var tag in recipeTags)
        {
            using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO ""Recipe_Tags"" (recipe_id, type_tag, meal_tag, cuisine_tag, dietary_tag) 
                VALUES (@recipe, @type, @meal, @cuisine, @dietary)",
            conn);
            insertCmd.Parameters.AddWithValue("@recipe", tag.RecipeId);
            insertCmd.Parameters.AddWithValue("@type", tag.Recipe_type ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@meal", tag.Meal ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@cuisine", tag.Cuisine ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@dietary", tag.Dietary);
            await insertCmd.ExecuteNonQueryAsync();
        }
    }

    public void Dispose() //async ValueTask DisposeAsync
    {
        ResetDatabase().Wait();
        SeedDatabase().Wait();
        GC.SuppressFinalize(this);
    }

    private class RecipeSeed
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Servings { get; set; }
        public string? CookTime { get; set; }
        public string? PrepTime { get; set; }
        public int? BakeTemp { get; set; }
        public string? Temp_unit { get; set; }
        public string? Description { get; set; }
    }

    private class SectionSeed
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string SectionName { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    private class IngredientListSeed
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int? SectionId { get; set; }
        public int IngredientOrder { get; set; }
        public int IngredientId { get; set; }
        public string? Quantity { get; set; }
        public int UnitId { get; set; }
        public string? Qualifier { get; set; } = "";
    }

    private class InstructionSeed
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int? SectionId { get; set; }
        public int StepNumber { get; set; }
        public string InstructionText { get; set; } = null!;
    }

    private class RecipeTagsSeed
    {
        public int RecipeId { get; set; }
        public string? Meal { get; set; }
        public string? Recipe_type { get; set; }
        public string? Cuisine { get; set; }
        public List<string> Dietary { get; set; } = [];
    }
}