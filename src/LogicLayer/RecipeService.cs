/* Business logic layer for existing recipes */

using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Models.enums;
using savorfolio_backend.Utils;

namespace savorfolio_backend.LogicLayer;

public class RecipeService(IRecipeRepository recipeRepository) : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;

    public async Task<List<RecipeDTO>> SearchRecipesAsync(RecipeFilterRequestDTO filter)
    {
        // parse recipe type string into enum
        var recipe_typeString = filter.Recipe_typeString;
        if (recipe_typeString != null)
        {
            var recipeTypeTag = EnumExtensions.ParseEnumMember<RecipeTypeTag>(recipe_typeString);
            filter.Recipe_type = recipeTypeTag;
        }
        // parse meal string into enum
        var mealString = filter.MealString;
        if (mealString != null)
        {
            var mealTag = EnumExtensions.ParseEnumMember<MealTag>(mealString);
            filter.Meal = mealTag;
        }
        // parse cuisine string into enum
        var cuisineString = filter.CuisineString;
        if (cuisineString != null)
        {
            var cuisineTag = EnumExtensions.ParseEnumMember<CuisineTag>(cuisineString);
            filter.Cuisine = cuisineTag;
        }
        
        var recipes = await _recipeRepository.ReturnRecipesFiltered(filter);
        return recipes;
    }
}