/* Will split the JSON into respective data transfer objects, call upon repositories,
starting with recipe repository to get generated recipe ID. 

Beginning process is extracting recipe table data to be able to get the recipe ID.
From there can split into respective data transfer objects, as the rest of the DTOs 
require the recipeID. Then can call other repositories. */

using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using Newtonsoft.Json.Linq;

namespace savorfolio_backend.LogicLayer;

public class AddRecipeService(
    IRecipeRepository recipeRepository,
    IIngListRepository ingListRepository,
    IInstructionsRepository instructionsRepository,
    ITagsRepository tagsRepository
) : IAddRecipeService
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;
    private readonly IIngListRepository _ingListRepository = ingListRepository;
    private readonly IInstructionsRepository _instructionsRepository = instructionsRepository;
    private readonly ITagsRepository _tagsRepository = tagsRepository;

    public async Task<OperationResult<int>> AddRecipeManually(JObject newRecipeContent) // modify this to return the full recipe to view?
    {
        // extract recipe information: name, servings, cook time, prep time, bake temp, temp unit
        var newRecipe = (newRecipeContent["RecipeSummary"]?.ToObject<RecipeDTO>()) ?? throw new InvalidOperationException("RecipeSummary section missing or invalid");

        // call recipeRepository.AddNewRecipe with the new recipe DTO (await)
        var newRecipeId = await _recipeRepository.AddNewRecipe(newRecipe);

        // build ingredient list DTO
        var ingList = (newRecipeContent["Ingredients"]?.ToObject<List<IngredientListDTO>>()) ?? throw new InvalidOperationException("Ingredients section missing or invalid");
        
        // call ingListRepository.AddNewRecipeIng with the new ingredient list DTO
        int ingAdded = _ingListRepository.AddNewRecipeIng(ingList, newRecipeId);

        // build instruction list DTO
        var instructions = (newRecipeContent["Instructions"]?.ToObject<List<InstructionDTO>>()) ?? throw new InvalidOperationException("Instructions section missing or invalid");
        
        // call instructionsRepository.AddNewRecipeIns with the new instruction DTO
        int insAdded = _instructionsRepository.AddNewRecipeIns(instructions, newRecipeId);

        // build recipe tags DTO
        var recipeTags = (newRecipeContent["RecipeTags"]?.ToObject<RecipeTagDTO>()) ?? throw new InvalidOperationException("RecipeTags section missing or invalid");

        // call tagsRepository.AddNewRecipeTags with the new tags DTO
        int tagsAdded = _tagsRepository.AddNewRecipeTags(recipeTags, newRecipeId);

        var result = new OperationResult<int>();

        // check if all entries were added successfully
        if (ingAdded > 0 & insAdded > 0 & tagsAdded > 0)
        {
            result.Success = true;
            result.Data = newRecipeId;
            result.Message = "Recipe added successfully";
        } else
        {
            result.Success = false;
            result.Message = "Failed to add recipe";
        }
        return result;
    }
}