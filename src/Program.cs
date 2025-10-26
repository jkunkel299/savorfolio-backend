using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.enums;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.DataAccess;
using savorfolio_backend.API;
using System.Text.RegularExpressions;
using savorfolio_backend.LogicLayer.WebScraper;

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
builder.Services.AddScoped<WebScraperService>();


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

// only added to debug web scraping in console
if (args.Contains("--scrape"))
{
    // wprm example: https://www.modernhoney.com/fall-chocolate-chip-spiced-cookie-levain-bakery-fall-cookie-knock-off/#wprm-recipe-container-24948 
    // tasty example: https://www.gimmesomeoven.com/mushroom-stroganoff
    // mv-create example: https://keytomylime.com/jiffy-cornbread-with-creamed-corn-recipe 
    // no pattern example: http://www.laundryinlouboutins.com/bundled-up-breakfast/  -- has no headings, only lists
    // https://myhomemaderecipe.com/recipes/chicken-tzatziki-rice-bake?utm_source=pinzk#:~:text=1%20lb%20boneless%20skinless%20chicken%20breast%20or%20thighs%2C%20diced%20into%20bite-size%20pieces&text=1%20cup%20uncooked%20long%20grain%20white%20rice&text=Prepare%20Baking%20Dish&text=Marinate%20Chicken
    // https://homeinspirediversity.com/2025/06/14/french-style-chicken-casserole-a-la-normande  -- totally unstructured, verbose
    // unlabeled, structured like a recipe https://www.williams-sonoma.com/recipe/crab-bisque.html?epik=dj0yJnU9aUozUlI2eFZ6LUYxaXdPV1lYanc4djBXLWlZZVUwUmwmcD0wJm49STl3dGlGc1hnT1lESmkwVzFMMk41USZ0PUFBQUFBR2o2SmJZ#:~:text=1%20Tbs&text=1%20Tbs&text=In%20a%20large%20soup%20pot%20over%20medium%20heat%2C%20melt%20the%20butter%20with%20the%20oil&text=Reduce%20the%20heat%20to%20low%20and%20stir%20in%20the%20crabmeat
    // somewhat labeled, mixed with tasty https://www.afamilyfeast.com/new-england-clam-chowder/#tasty-recipes-22643
    // https://www.closetcooking.com/crispy-parmesan-roast-potatoes
    // https://fashionablefoods.com/2014/06/27/2014618roasted-chili-lime-cod
    Console.WriteLine("Console mode activated.");
    using var scope = app.Services.CreateScope();
    var scraper = scope.ServiceProvider.GetRequiredService<WebScraperService>();
    var match = await scraper.RunScraper("https://fashionablefoods.com/2014/06/27/2014618roasted-chili-lime-cod");
    // https://homeinspirediversity.com/2025/06/14/french-style-chicken-casserole-a-la-normande");
    Console.WriteLine($"Return: \n\n{match}\n\n");
    return; // prevent app.Run() from starting the web server
}

app.Run();

public partial class Program { }