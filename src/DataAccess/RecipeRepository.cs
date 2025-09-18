using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class RecipeRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<Recipe>> ReturnAllRecipes()
    {
        var result = await _context.Recipes
            .OrderBy(i => i.Name)
            .ToListAsync();

        return result;
    }

    public async Task<List<RecipeDTO>> ReturnRecipesFiltered(RecipeFilterRequestDTO filter)
    {
        var query = _context.Recipes.AsQueryable();

        // filter to include ingredients
        if (filter.IncludeIngredients is { Count: > 0 })
        {
            var ingredientIds = filter.IncludeIngredients;

            query = query.Where(r =>
                ingredientIds.All(ingId =>
                    r.IngredientLists.Any(ri => ri.IngredientId == ingId)));
        }

        // filter to exclude ingredients
        if (filter.ExcludeIngredients is { Count: > 0 })
        {
            var ingredientIds = filter.ExcludeIngredients;

            query = query.Where(r =>
                !ingredientIds.All(ingId =>
                    r.IngredientLists.Any(ri => ri.IngredientId == ingId)));
        }

        // Shape into RecipeDTO and IngredientListDTO
        var result = query
            .Select(r => new RecipeDTO
            {
                Id = r.Id,
                Name = r.Name,
                Servings = r.Servings,
                CookTime = r.CookTime,
                PrepTime = r.PrepTime,
                Ingredients = r.IngredientLists
                    .Select(ri => new IngredientListDTO
                    {
                        Id = ri.IngredientId,
                        RecipeId = ri.RecipeId,
                        IngredientId = ri.Ingredient.Id,
                        IngredientName = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        UnitId = ri.UnitId,
                        UnitName = ri.Unit.Name,
                    }).ToList()
            });
            
        return await result.ToListAsync();

    }
}