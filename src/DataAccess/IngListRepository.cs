/* Data access layer to Ingredient_Lists entity */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class IngListRepository(AppDbContext context) : IIngListRepository
{
    private readonly AppDbContext _context = context;

    public int AddNewRecipeIng(List<IngredientListDTO> ingredientsData, int recipeId)
    {
        // for each ingredient in the list, create a new row in the IngredientList table
        foreach (IngredientListDTO ingredient in ingredientsData)
        {
            var newIngredient = new IngredientList
            {
                RecipeId = recipeId,
                IngredientOrder = ingredient.IngredientOrder,
                // SectionId = ingredient.SectionId,
                Quantity = ingredient.Quantity,
                UnitId = ingredient.UnitId,
                IngredientId = ingredient.IngredientId,
                Qualifier = ingredient.Qualifier
            };

            _context.IngredientLists.Add(newIngredient);
        }

        var result = _context.SaveChanges();
        return result;
    }

    public Task<List<IngredientListDTO>> GetIngredientsByRecipeAsync(int recipeId)
    {
        var result = _context.IngredientLists
            .Where(i => i.RecipeId == recipeId)
            .Select(e => new IngredientListDTO
            {
                Id = e.Id,
                RecipeId = e.RecipeId,
                IngredientOrder = e.IngredientOrder,
                IngredientId = e.IngredientId,
                IngredientName = e.Ingredient.Name,
                IngNamePlural = e.Ingredient.PluralName,
                Quantity = e.Quantity,
                UnitId = e.UnitId,
                UnitName = e.Unit.Name,
                UnitNamePlural = e.Unit.PluralName!,
                Qualifier = e.Qualifier,
            })
            .OrderBy(e => e.IngredientOrder)
            .ToListAsync();

        return result;
    }
}