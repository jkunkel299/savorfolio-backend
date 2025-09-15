using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.DataAccess;
using savorfolio_backend.API;

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
// builder.Services.AddScoped<IngredientRepository>();
// Recipe services 
builder.Services.AddScoped<RecipeService>();
// builder.Services.AddScoped<RecipeRepository>(); 

var app = builder.Build();

app.MapIngredientApi();
app.MapRecipeApi();

app.Run();
