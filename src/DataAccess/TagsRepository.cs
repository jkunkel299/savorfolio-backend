/* Data access layer to Instructions entity */

using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Models.enums;
using savorfolio_backend.Utils;

namespace savorfolio_backend.DataAccess;

public class TagsRepository(AppDbContext context) : ITagsRepository
{
    private readonly AppDbContext _context = context;

    public int AddNewRecipeTags(RecipeTagDTO recipeTags, int recipeId)
    {
        var newTag = new RecipeTag
        {
            RecipeId = recipeId,
            Meal = recipeTags.Meal,
            Recipe_type = recipeTags.Recipe_type,
            Cuisine = recipeTags.Cuisine,
            Dietary = [.. recipeTags.Dietary.Select(d => d.GetEnumMemberValue())],
        };
        _context.RecipeTags.Add(newTag);

        var result = _context.SaveChanges();
        return result;
    }

    public TagStringsDTO GetTagsByRecipe(int recipeId)
    {
        // get the record that matches the given recipeId, or throw exception
        var entity =
            _context.RecipeTags.FirstOrDefault(t => t.RecipeId == recipeId)
            ?? throw new Exception("Could not find recipe tags for the given recipe id");

        // since CuisineTag uses EnumMember for "Middle Eastern", parse appropriately
        var cuisineTag =
            entity.Cuisine != null
                ? EnumExtensions.ParseEnumMember<CuisineTag>(entity.Cuisine.ToString()!)
                : null;
        CuisineTag? cuisine = cuisineTag;

        // get each dietary tag from the text[] field in the postgres database
        List<DietaryTag> dietaryTags =
        [
            .. entity
                .Dietary.Select(EnumExtensions.ParseEnumMember<DietaryTag>)
                .Where(t => t.HasValue)
                .Select(t => t!.Value),
        ];
        // initialize string array of dietary tags
        List<string> dietaryStrings = [];
        // for each dietary tag in the list, get the enum member value as string and add to the array
        foreach (var tag in dietaryTags)
        {
            var stringTag = EnumExtensions.GetEnumMemberValue(tag);
            dietaryStrings.Add(stringTag);
        }

        // build the TagStringsDTO (similar to RecipeTagDTO, but stringified)
        var result = new TagStringsDTO
        {
            RecipeId = entity!.RecipeId,
            // get the enum member value for Meal, Recipe_type, and cuisine as a string
            Meal = entity.Meal.HasValue
                ? EnumExtensions.GetEnumMemberValue(entity.Meal.Value)
                : null,
            Recipe_type = entity.Recipe_type.HasValue
                ? EnumExtensions.GetEnumMemberValue(entity.Recipe_type.Value)
                : null,
            Cuisine = cuisine.HasValue ? EnumExtensions.GetEnumMemberValue(cuisine.Value) : null,
            Dietary = dietaryStrings,
        };

        return result;
    }
}
