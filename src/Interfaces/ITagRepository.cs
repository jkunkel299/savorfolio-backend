using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface ITagsRepository
{
    int AddNewRecipeTags(RecipeTagDTO recipeTags, int recipeId);
    TagStringsDTO GetTagsByRecipe(int recipeId);
}
