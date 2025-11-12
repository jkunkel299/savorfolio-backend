/* Data access layer to Recipe entity */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class RecipeRepository(AppDbContext context) : IRecipeRepository
{
    private readonly AppDbContext _context = context;

    // queries recipe database for recipe by ID
    public async Task<RecipeDTO> ReturnRecipeByIdAsync(int recipeId)
    {
        var result = await _context
            .Recipes.Where(r => r.Id == recipeId)
            .Select(r => new RecipeDTO
            {
                Id = r.Id,
                Name = r.Name,
                Servings = r.Servings,
                CookTime = r.CookTime,
                PrepTime = r.PrepTime,
                BakeTemp = r.BakeTemp,
                Temp_unit = r.Temp_unit,
                Description = r.Description,
            })
            .SingleOrDefaultAsync();

        return result!;
    }

    // queries recipe database for recipes, including filtering
    public async Task<List<RecipeDTO>> ReturnRecipesFiltered(RecipeFilterRequestDTO filter)
    {
        var query = _context.Recipes.AsQueryable();

        // filter to include ingredients
        if (filter.IncludeIngredients is { Count: > 0 })
        {
            var ingredientIds = filter.IncludeIngredients;

            query = query.Where(r =>
                ingredientIds.All(ingId => r.IngredientLists.Any(ri => ri.IngredientId == ingId))
            );
        }

        // filter to exclude ingredients
        if (filter.ExcludeIngredients is { Count: > 0 })
        {
            var ingredientIds = filter.ExcludeIngredients;

            query = query.Where(r =>
                !ingredientIds.All(ingId => r.IngredientLists.Any(ri => ri.IngredientId == ingId))
            );
        }

        // filter by recipe type tag
        if (filter.Recipe_type != null)
        {
            var recipe_type = filter.Recipe_type;

            query = query.Where(r => r.RecipeTags!.Recipe_type == recipe_type);
        }

        // filter by meal tag
        if (filter.Meal != null)
        {
            var meal = filter.Meal;

            query = query.Where(r => r.RecipeTags!.Meal == meal);
        }

        // filter by cuisine tag
        if (filter.Cuisine != null)
        {
            var cuisine = filter.Cuisine;

            query = query.Where(r => r.RecipeTags!.Cuisine == cuisine);
        }

        // filter by dietary tags
        if (filter.Dietary is { Count: > 0 })
        {
            var dietaryTags = filter.Dietary;

            query = query.Where(r => dietaryTags.All(diet => r.RecipeTags!.Dietary.Contains(diet)));
        }

        // Shape into RecipeDTO and IngredientListDTO
        var result = query.Select(r => new RecipeDTO
        {
            Id = r.Id,
            Name = r.Name,
            Servings = r.Servings,
            CookTime = r.CookTime,
            PrepTime = r.PrepTime,
            BakeTemp = r.BakeTemp,
            Temp_unit = r.Temp_unit,
            Description = r.Description,
            Ingredients = r
                .IngredientLists.OrderBy(ri => ri.IngredientOrder)
                .Select(ri => new IngredientListDTO
                {
                    Id = ri.Id,
                    RecipeId = ri.RecipeId,
                    IngredientId = ri.Ingredient.Id,
                    IngredientOrder = ri.IngredientOrder,
                    IngredientName = ri.Ingredient.Name,
                    Quantity = ri.Quantity,
                    Qualifier = ri.Qualifier,
                    UnitId = ri.UnitId,
                    UnitName = ri.Unit.Name,
                })
                .ToList(),
        });

        return await result.ToListAsync();
    }

    // Add new recipe to database, needs to return at least the recipe ID
    public async Task<int> AddNewRecipeAsync(RecipeDTO recipeData)
    {
        // generate new recipe record to be input into Recipe table
        var newRecipe = new Recipe
        {
            Name = recipeData.Name,
            Servings = recipeData.Servings,
            CookTime = recipeData.CookTime,
            PrepTime = recipeData.PrepTime,
            BakeTemp = recipeData.BakeTemp,
            Temp_unit = recipeData.Temp_unit,
            Description = recipeData.Description,
        };

        // add new recipe to Recipe table and save changes
        _context.Recipes.Add(newRecipe);
        await _context.SaveChangesAsync();

        return newRecipe.Id;
    }
}
