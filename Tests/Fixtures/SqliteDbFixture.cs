using Tests.Helpers;
using savorfolio_backend.Data;
using Microsoft.Data.Sqlite;  
using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace Tests.Fixtures;

public class SqliteDbFixture : IDisposable
{
    public AppDbContext Context { get; }

    public SqliteDbFixture()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        Context = new AppDbContext(options);

        Context.Database.EnsureCreated();

        Context.IngredientVariants.RemoveRange(Context.IngredientVariants);
        Context.IngredientTypes.RemoveRange(Context.IngredientTypes);
        Context.Units.RemoveRange(Context.Units);
        Context.Recipes.RemoveRange(Context.Recipes);
        Context.IngredientLists.RemoveRange(Context.IngredientLists);
        Context.SaveChanges();

        string ingTypeFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Types1.json");
        string ingVariantFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Variants1.json");
        string unitsFilePath = TestFileHelper.GetProjectPath("SeedData/Units.json");
        string recipeFilePath = TestFileHelper.GetProjectPath("SeedData/Recipe.json");
        string ingListFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Lists.json");

        InMemoryDbSeeder.SeedFromJson<IngredientType>(Context, ingTypeFilePath);
        InMemoryDbSeeder.SeedVariantsFromJson(Context, ingVariantFilePath);
        InMemoryDbSeeder.SeedFromJson<Unit>(Context, unitsFilePath);
        InMemoryDbSeeder.SeedFromJson<Recipe>(Context, recipeFilePath);
        InMemoryDbSeeder.SeedIngredientListsFromJson(Context, ingListFilePath);
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}