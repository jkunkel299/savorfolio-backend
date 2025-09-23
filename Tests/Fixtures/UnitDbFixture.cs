using Tests.Helpers;
using savorfolio_backend.Data;
using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Models;
namespace Tests.Fixtures;

public class UnitDbFixture : IDisposable
{
    public AppDbContext Context { get; }

    public UnitDbFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("UnitTestsDb")
            .EnableSensitiveDataLogging()
            .Options;

        Context = new AppDbContext(options);

        Context.IngredientVariants.RemoveRange(Context.IngredientVariants);
        Context.IngredientTypes.RemoveRange(Context.IngredientTypes);
        Context.SaveChanges();
        
        string ingTypeFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Types1.json");
        string ingVariantFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Variants1.json");

        InMemoryDbSeeder.SeedFromJson<IngredientType>(Context, ingTypeFilePath);
        InMemoryDbSeeder.SeedVariantsFromJson(Context, ingVariantFilePath);
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}