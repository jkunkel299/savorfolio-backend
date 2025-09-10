using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.DataAccess; // see if this is needed

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDatabase"));
        // builder.Configuration.GetConnectionString("NeonDatabase"),
        // npgsqlOptions =>
        // {
        //     npgsqlOptions.MapEnum<CuisineTag>("cuisine");
        //     npgsqlOptions.MapEnum<DietaryTag>("dietary");
        //     npgsqlOptions.MapEnum<MealTag>("meal");
        //     npgsqlOptions.MapEnum<RecipeTypeTag>("recipe_type");
        //     npgsqlOptions.MapEnum<TempUnitsTag>("temp_units");
        // });
});

// Register services
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<IngredientRepository>(); // see if this is needed

var app = builder.Build();

app.MapGet("/api/ingredients/search", async (
    string term,
    IngredientService ingredientService) =>
{
    if (string.IsNullOrWhiteSpace(term))
    {
        return Results.BadRequest("Search term is required.");
    }

    var results = await ingredientService.SearchIngredientsAsync(term);
    return Results.Ok(results);
});

app.Run();
