using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using Tests.Helpers;

namespace Tests.Fixtures;

public class SqliteDbFixture : IDisposable
{
    public AppDbContext Context { get; }

    public SqliteDbFixture()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;

        Context = new AppDbContext(options);

        Context.Database.EnsureCreated();

        Context.IngredientVariants.RemoveRange(Context.IngredientVariants);
        Context.IngredientTypes.RemoveRange(Context.IngredientTypes);
        Context.Units.RemoveRange(Context.Units);
        Context.Recipes.RemoveRange(Context.Recipes);
        Context.RecipeSections.RemoveRange(Context.RecipeSections);
        Context.IngredientLists.RemoveRange(Context.IngredientLists);
        Context.Instructions.RemoveRange(Context.Instructions);
        Context.RecipeTags.RemoveRange(Context.RecipeTags);
        Context.UserRecipes.RemoveRange(Context.UserRecipes);
        Context.Users.RemoveRange(Context.Users);
        Context.SaveChanges();

        string ingTypeFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Types1.json");
        string ingVariantFilePath = TestFileHelper.GetProjectPath(
            "SeedData/Ingredient_Variants1.json"
        );
        string unitsFilePath = TestFileHelper.GetProjectPath("SeedData/Units.json");
        string recipeFilePath = TestFileHelper.GetProjectPath("SeedData/Recipe.json");
        string sectionsFilePath = TestFileHelper.GetProjectPath("SeedData/RecipeSections.json");
        string ingListFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Lists.json");
        string insListFilePath = TestFileHelper.GetProjectPath("SeedData/Instructions.json");
        string tagsFilePath = TestFileHelper.GetProjectPath("SeedData/RecipeTags.json");
        string usersFilePath = TestFileHelper.GetProjectPath("SeedData/Users.json");
        string userRecipesFilePath = TestFileHelper.GetProjectPath("SeedData/UserRecipes.json");

        InMemoryDbSeeder.SeedFromJson<IngredientType>(Context, ingTypeFilePath);
        InMemoryDbSeeder.SeedVariantsFromJson(Context, ingVariantFilePath);
        InMemoryDbSeeder.SeedFromJson<Unit>(Context, unitsFilePath);
        InMemoryDbSeeder.SeedFromJson<Recipe>(Context, recipeFilePath);
        InMemoryDbSeeder.SeedSectionsFromJson(Context, sectionsFilePath);
        InMemoryDbSeeder.SeedIngredientListsFromJson(Context, ingListFilePath);
        InMemoryDbSeeder.SeedInstructionsFromJson(Context, insListFilePath);
        InMemoryDbSeeder.SeedTagsFromJson(Context, tagsFilePath);
        InMemoryDbSeeder.SeedFromJson<User>(Context, usersFilePath);
        InMemoryDbSeeder.SeedUserRecipesFromJson(Context, userRecipesFilePath);
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
