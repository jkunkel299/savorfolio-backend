/* Data access layer to Instructions entity */

using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;
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
            Dietary = [.. recipeTags.Dietary.Select(d => d.GetEnumMemberValue())]
        };
        _context.RecipeTags.Add(newTag);

        var result = _context.SaveChanges();
        return result;
    }
}