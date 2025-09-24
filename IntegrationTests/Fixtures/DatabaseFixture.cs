using System.Data;
using System.Runtime.CompilerServices;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using savorfolio_backend.Data;
using savorfolio_backend.Models;

namespace IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    public AppDbContext Context { get; }

    public DatabaseFixture()
    {
        // Add connection string from environment variables
        Env.Load();
        ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TESTING_DATABASE_CONNECTION")
            ?? throw new InvalidOperationException("ConnectionStrings__TESTING_DATABASE_CONNECTION not found in .env");

        // Configure TestDbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        Context = new AppDbContext(options);

        // // Reset and seed database when fixture is created 
        // -- will work on implementing when tests will actually be manipulating the database, not just reading from it
        // ResetDatabase().Wait();
        // SeedDatabase().Wait();
    }

    // -- will work on implementing when tests will actually be manipulating the database, not just reading from it
    private async Task ResetDatabase()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        // List of certain tables to reset
        var tablesToReset = new[]
        {
            "Recipe",
            "Ingredient_Lists",
            "Instructions",
            "Notes",
            "Recipe_Sections",
            "Recipe_Tags"
        };

        // Reset tables in list
        foreach (var table in tablesToReset)
        {
            var cmd = new NpgsqlCommand(
                $"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;", conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // -- will work on implementing when tests will actually be manipulating the database, not just reading from it
    private async Task SeedDatabase()
    {
        Context.Recipes.AddRange(
            new Recipe { Name = "Chicken Ragout", Servings = 4, CookTime = "20 minutes", PrepTime = "10 minutes" },
            new Recipe { Name = "Fall Spice Chocolate Chip Cookies", Servings = 8, CookTime = "10 minutes", PrepTime = "15 minutes", BakeTemp = 400, Temp_unit = "F" }
        );

        await Context.SaveChangesAsync();

        Context.IngredientLists.AddRange(
            new IngredientList { RecipeId = 1, Quantity = "1.5", UnitId = 12, IngredientId = 143 },
            new IngredientList { RecipeId = 1, Quantity = "1", UnitId = 135, IngredientId = 221 },
            new IngredientList { RecipeId = 1, Quantity = "3 1/3", UnitId = 2, IngredientId = 24 },
            new IngredientList { RecipeId = 1, IngredientId = 250 },
            new IngredientList { RecipeId = 1, IngredientId = 189 },
            new IngredientList { RecipeId = 1, Quantity = "2", UnitId = 2, IngredientId = 54 },
            new IngredientList { RecipeId = 1, Quantity = "1.5", UnitId = 4, IngredientId = 251 },
            new IngredientList { RecipeId = 1, Quantity = "2", UnitId = 2, IngredientId = 252 },
            new IngredientList { RecipeId = 1, Quantity = "3", UnitId = 2, IngredientId = 2 },
            new IngredientList { RecipeId = 1, Quantity = "1", IngredientId = 162 },
            new IngredientList { RecipeId = 1, Quantity = "0.5", UnitId = 4, IngredientId = 50 },
            new IngredientList { RecipeId = 1, IngredientId = 94 },
            new IngredientList { RecipeId = 1, Quantity = "12", UnitId = 2, IngredientId = 24 },
            new IngredientList { RecipeId = 2, Quantity = "0.5", UnitId = 4, IngredientId = 200 },
            new IngredientList { RecipeId = 2, Quantity = "0.5", UnitId = 4, IngredientId = 199 },
            new IngredientList { RecipeId = 2, Quantity = "1", IngredientId = 159 },
            new IngredientList { RecipeId = 2, Quantity = "2", UnitId = 2, IngredientId = 205 },
            new IngredientList { RecipeId = 2, Quantity = "2", UnitId = 4, IngredientId = 54 },
            new IngredientList { RecipeId = 2, Quantity = "1", UnitId = 1, IngredientId = 97 },
            new IngredientList { RecipeId = 2, Quantity = "1", UnitId = 1, IngredientId = 253 },
            new IngredientList { RecipeId = 2, Quantity = "0.5", UnitId = 1, IngredientId = 250 },
            new IngredientList { RecipeId = 2, Quantity = "1", UnitId = 1, IngredientId = 185 },
            new IngredientList { RecipeId = 2, Quantity = "1.25", UnitId = 1, IngredientId = 188 },
            new IngredientList { RecipeId = 2, Quantity = "1/8", UnitId = 1, IngredientId = 187 },
            new IngredientList { RecipeId = 2, Quantity = "1 1/2", UnitId = 4, IngredientId = 38 }
        );

        Context.Instructions.AddRange(
            new Instruction { RecipeId = 1, StepNumber = 1, InstructionText = "Chop the onion fine." },
            new Instruction { RecipeId = 1, StepNumber = 2, InstructionText = "Cut the chicken (breast or thigh) into small pieces" },
            new Instruction { RecipeId = 1, StepNumber = 3, InstructionText = "Fry chicken in butter in at least two portions. Season with salt and pepper. Remove after approx. 3 minutes, once the chicken has a dark golden color" },
            new Instruction { RecipeId = 1, StepNumber = 4, InstructionText = "Saute the onions in the same skillet until soft (translucent)" },
            new Instruction { RecipeId = 1, StepNumber = 5, InstructionText = "Powder flour over the onions and cook 1-2 minutes" },
            new Instruction { RecipeId = 1, StepNumber = 6, InstructionText = "Slowly add chicken broth, stirring until smooth." },
            new Instruction { RecipeId = 1, StepNumber = 7, InstructionText = "Spice the gravy with lemon juice, white wine, salt, and pepper. Simmer 5 mintues." },
            new Instruction { RecipeId = 1, StepNumber = 8, InstructionText = "Add chicken and cook to 165°F. Remove from heat." },
            new Instruction { RecipeId = 1, StepNumber = 9, InstructionText = "Mix egg yolk and heavy cream in a separate bowl. Add to the chicken and gravy and mix well." },
            new Instruction { RecipeId = 1, StepNumber = 10, InstructionText = "(Optional) Top with chives." },
            new Instruction { RecipeId = 1, StepNumber = 11, InstructionText = "Serve with rice." },
            new Instruction { RecipeId = 2, StepNumber = 1, InstructionText = "Preheat oven to 400°F" },
            new Instruction { RecipeId = 2, StepNumber = 2, InstructionText = "In a large mixing bowl, beat butter, brown sugar, and sugar for 4 minutes until light and fluffy." },
            new Instruction { RecipeId = 2, StepNumber = 3, InstructionText = "Add molasses and egg and mix for 1 minute longer." },
            new Instruction { RecipeId = 2, StepNumber = 4, InstructionText = "Fold in flour, baking soda, corn starch, salt, cinnamon, ginger, nutmeg, cloves, and chocolate chips." },
            new Instruction { RecipeId = 2, StepNumber = 5, InstructionText = "Roll into 4-ounce, 5-ounce, or 6-ounce balls. Place on a parchment paper-lined baking sheet. I prefer to use light-colored baking sheets." },
            new Instruction { RecipeId = 2, StepNumber = 6, InstructionText = "Bake for 8-10 minutes. The cookies will be slightly underdone when you remove them from the oven. Let the cookies sit for 10-15 minutes before moving them from the baking sheet to a wire cooling rack." }
        );

        Context.RecipeTags.AddRange(
            new RecipeTag { RecipeId = 1, Recipe_type = RecipeTypeTag.Main, Meal = MealTag.Dinner, Cuisine = CuisineTag.Bavarian },
            new RecipeTag { RecipeId = 2, Recipe_type = RecipeTypeTag.Dessert, Meal = MealTag.Dinner, Dietary = DietaryTag.NutFree }
        );

        await Context.SaveChangesAsync();
        // await using var conn = new NpgsqlConnection(ConnectionString);
        // await conn.OpenAsync();

        // var seed = new NpgsqlCommand(@"
        //     -- seed Recipes
        //     INSERT INTO Recipe (Name, Servings, CookTime, PrepTime, BakeTemp, Temp_unit)
        //     VALUES  ('Chicken Ragout', 4, '20 minutes', '10 minutes', NULL, NULL),
        //             ('Fall Spice Chocolate Chip Cookies', 8, '10 minutes', '15 minutes', 400, 'F');

        //     -- seed Ingredient_Lists
        //     INSERT INTO Ingredient_List (RecipeId, SectionId, Quantity, UnitId, IngredientId)
        //     VALUES  (1, NULL, '1.5', 12, 143),
        //             (1, NULL, '1', 35, 221),
        //             (1, NULL, '3 1/3', 2, 24),
        //             (1, NULL, NULL, NULL, 250),
        //             (1, NULL, NULL, NULL, 189),
        //             (1, NULL, '2', 2, 54),
        //             (1, NULL, '1.5', 4, 251),
        //             (1, NULL, '2', 2, 252),
        //             (1, NULL, '3', 2, 2),
        //             (1, NULL, '1', NULL, 162),
        //             (1, NULL, '0.5', 4, 50),
        //             (1, NULL, NULL, NULL, 94),
        //             (1, NULL, '12', 2, 24),
        //             (2, NULL, '0.5', 4, 200),
        //             (2, NULL, '0.5', 24, 199),
        //             (2, NULL, '1', NULL, 159),
        //             (2, NULL, '2', 2, 205),
        //             (2, NULL, '2', 4, 54),
        //             (2, NULL, '1', 1, 97),
        //             (2, NULL, '1', 1, 253),
        //             (2, NULL, '0.5', 1, 250),
        //             (2, NULL, '1', 1, 185),
        //             (2, NULL, '1.25' 1, 188),
        //             (2, NULL, '1/8', 1, 187),
        //             (2, NULL, '1 1/2', 4, 38);

        // -- seed Instructions
        // INSERT INTO Instruction (RecipeId, SectionId, StepNumber, InstructionText)
        // VALUES  (1, NULL, 1, 'Chop the onion fine.'),
        //         (1, NULL, 2, 'Cut the chicken (breast or thigh) into small pieces'),
        //         (1, NULL, 3, 'Fry chicken in butter in at least two portions. Season with salt and pepper. Remove after approx. 3 minutes, once the chicken has a dark golden color')
        //         (1, NULL, 4, 'Saute the onions in the same skillet until soft (translucent)'),
        //         (1, NULL, 5, 'Powder flour over the onions and cook 1-2 minutes'),
        //         (1, NULL, 6, 'Slowly add chicken broth, stirring until smooth.'),
        //         (1, NULL, 7, 'Spice the gravy with lemon juice, white wine, salt, and pepper. Simmer 5 mintues.'),
        //         (1, NULL, 8, 'Add chicken and cook to 165°F. Remove from heat.'),
        //         (1, NULL, 9, 'Mix egg yolk and heavy cream in a separate bowl. Add to the chicken and gravy and mix well.'),
        //         (1, NULL, 10, '(Optional) Top with chives.'),
        //         (1, NULL, 11, 'Serve with rice.'),
        //         (2, NULL, 1, 'Preheat oven to 400°F'),
        //         (2, NULL, 2, 'In a large mixing bowl, beat butter, brown sugar, and sugar for 4 minutes until light and fluffy.'),
        //         (2, NULL, 3, 'Add molasses and egg and mix for 1 minute longer.'),
        //         (2, NULL, 4, 'Fold in flour, baking soda, corn starch, salt, cinnamon, ginger, nutmeg, cloves, and chocolate chips.'),
        //         (2, NULL, 5, 'Roll into 4-ounce, 5-ounce, or 6-ounce balls. Place on a parchment paper-lined baking sheet. I prefer to use light-colored baking sheets.'),
        //         (2, NULL, 5, 'Bake for 8-10 minutes. The cookies will be slightly underdone when you remove them from the oven. Let the cookies sit for 10-15 minutes before moving them from the baking sheet to a wire cooling rack.');

        // -- seed Recipe_Tags
        // INSERT INTO Recipe_Tags (recipe_id, type_tag, meal_tag, cuisine_tag, dietary_tag)
        // VALUES  (1, 'Main', 'Dinner', 'Bavarian', NULL),
        //         (1, 'Dessert, 'Dessert', NULL, 'Nut-Free');
        // ", conn);

        // await seed.ExecuteNonQueryAsync();
    }

    public void Dispose () //async ValueTask DisposeAsync
    {
        // await using var conn = new NpgsqlConnection(ConnectionString);
        // await conn.OpenAsync();

        // // List of certain tables to reset
        // var tablesToReset = new[]
        // {
        //     "Recipe",
        //     "Ingredient_Lists",
        //     "Instructions",
        //     "Notes",
        //     "Recipe_Sections",
        //     "Recipe_Tags"
        // };

        // // Reset tables in list
        // foreach (var table in tablesToReset)
        // {
        //     var cmd = new NpgsqlCommand(
        //         $"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;", conn);
        //     await cmd.ExecuteNonQueryAsync();
        // }

        GC.SuppressFinalize(this);

    }
}