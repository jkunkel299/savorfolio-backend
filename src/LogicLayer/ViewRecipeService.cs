/* calls upon recipe, ingredients list, instructions, tags, (etc.) repositories
to search by recipe ID. Combines the results into a conglomerate JSON for the 
API controller to send to the frontend for display */
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class ViewRecipeService(
    IRecipeRepository recipeRepository,
    IIngListRepository ingListRepository,
    IInstructionsRepository instructionsRepository,
    ITagsRepository tagsRepository
) : IViewRecipeService
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;
    private readonly IIngListRepository _ingListRepository = ingListRepository;
    private readonly IInstructionsRepository _instructionsRepository = instructionsRepository;
    private readonly ITagsRepository _tagsRepository = tagsRepository;

    public async Task<FullRecipeDTO> CompileRecipeAsync(int recipeId)
    {
        var recipeSummary = await _recipeRepository.ReturnRecipeByIdAsync(recipeId);
        var recipeTags = _tagsRepository.GetTagsByRecipe(recipeId);
        var ingredientsList = await _ingListRepository.GetIngredientsByRecipeAsync(recipeId);
        var instructionsList = await _instructionsRepository.GetInstructionsByRecipeAsync(recipeId);

        var compiledRecipe = new FullRecipeDTO
        {
            RecipeId = recipeId,
            RecipeSummary = recipeSummary,
            RecipeTags = recipeTags,
            Ingredients = ingredientsList,
            Instructions = instructionsList
        };

        return compiledRecipe;
    }
}