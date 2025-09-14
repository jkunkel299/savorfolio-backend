using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add connection string from environment variables
Env.Load();
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DATABASE_CONNECTION");

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
    connectionString,
    o =>
    { // Map database enum types
        o.MapEnum<CuisineTag>("cuisine");
        o.MapEnum<DietaryTag>("dietary");
        o.MapEnum<MealTag>("meal");
        o.MapEnum<RecipeTypeTag>("recipe_type");
        o.MapEnum<TempUnitsTag>("temp_units");
    }
));

// ** Register services **
// Ingredient services
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<IngredientRepository>();
// Recipe services 
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<RecipeRepository>(); 

var app = builder.Build();

// get ingredients by term
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

// get all recipes
app.MapGet("/api/recipes", async (
    RecipeService recipeService) =>
{
    var results = await recipeService.ReturnRecipesAsync();
    return Results.Ok(results);
});

app.Run();
