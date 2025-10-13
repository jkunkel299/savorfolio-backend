using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.enums;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.DataAccess;
using savorfolio_backend.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add connection string from environment variables
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
Env.Load($".env.{environment.ToLower()}", new LoadOptions(setEnvVars: true, clobberExistingVars: true, onlyExactPath: true));
// default to .env if not testing
Env.Load(".env", new LoadOptions(setEnvVars: true, clobberExistingVars: false, onlyExactPath: true));
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
// Ingredient Services
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
// Unit ServiceS
builder.Services.AddScoped<IUnitsService, UnitsService>();
builder.Services.AddScoped<IUnitsRepository, UnitsRepository>();
// Recipe Services 
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IAddRecipeService, AddRecipeService>();
builder.Services.AddScoped<IViewRecipeService, ViewRecipeService>();
// Ingredient List Services
builder.Services.AddScoped<IIngListRepository, IngListRepository>();
// Instructions Services
builder.Services.AddScoped<IInstructionsRepository, InstructionsRepository>();
//Tags Services
builder.Services.AddScoped<ITagsRepository, TagsRepository>();


var app = builder.Build();

app.UseCors("AllowAll");

// search for ingredients
app.MapIngredientApi();
// search for units
app.MapUnitApi();
// return recipe tag enumerators
app.GetMealTags();
app.GetRecipeTypeTags();
app.GetCuisineTags();
app.GetDietaryTags();
// view a recipe
app.MapRecipeById();
// search for a recipe by filters
app.MapRecipeSearch();
// add a new recipe manually
app.MapManualRecipe();





// add health endpoint for E2E testing with Playwright
app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();

public partial class Program { }