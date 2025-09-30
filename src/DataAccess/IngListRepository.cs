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
}