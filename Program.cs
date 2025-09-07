using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDatabase"),
        npgsqlOptions => npgsqlOptions.MapEnum<CuisineTag>("cuisine"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDatabase"),
        npgsqlOptions => npgsqlOptions.MapEnum<DietaryTag>("dietary"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDatabase"),
        npgsqlOptions => npgsqlOptions.MapEnum<MealTag>("meal"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDatabase"),
        npgsqlOptions => npgsqlOptions.MapEnum<RecipeTypeTag>("recipe_type"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonDatabase"),
        npgsqlOptions => npgsqlOptions.MapEnum<TempUnitsTag>("temp_units"));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();
/* 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
 */
app.Run();

/* record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
} */
