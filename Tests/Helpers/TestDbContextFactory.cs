using savorfolio_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests.Helpers;

public static class TestDbContextFactory{
    // public static AppDbContext CreateWithSeededData()
    // {
    //     var options = new DbContextOptionsBuilder<AppDbContext>()
    //         .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //         .EnableSensitiveDataLogging()
    //         .Options;

    //     var context = new AppDbContext(options);

    //     string ingTypeFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Types1.json");
    //     string ingVariantFilePath = TestFileHelper.GetProjectPath("SeedData/Ingredient_Variants1.json");

    //     InMemoryDbSeeder.SeedFromJson(context, ingTypeFilePath);
    //     InMemoryDbSeeder.SeedFromJson(context, ingVariantFilePath);

    //     return context;
    // }
}