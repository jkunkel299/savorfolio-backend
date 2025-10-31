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
    ISectionsRepository sectionsRepository,
    IIngListRepository ingListRepository,
    IInstructionsRepository instructionsRepository,
    ITagsRepository tagsRepository
) : IAddRecipeService
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;
    private readonly ISectionsRepository _sectionsRepository = sectionsRepository;
    private readonly IIngListRepository _ingListRepository = ingListRepository;
    private readonly IInstructionsRepository _instructionsRepository = instructionsRepository;
    private readonly ITagsRepository _tagsRepository = tagsRepository;

    public async Task<OperationResult<int>> AddRecipeManually(JObject newRecipeContent)
    {
        // extract recipe information: name, servings, cook time, prep time, bake temp, temp unit
        var newRecipe = (newRecipeContent["recipeSummary"]?.ToObject<RecipeDTO>()) ?? throw new InvalidOperationException("RecipeSummary section missing or invalid");

        // call recipeRepository.AddNewRecipe with the new recipe DTO (await)
        var newRecipeId = await _recipeRepository.AddNewRecipe(newRecipe);

        // build sections list DTO or set as empty list
        var sectionsList = (newRecipeContent["sections"]?.ToObject<List<SectionDTO>>()) ?? [];

        // if sections, call sectionsRepository.AddNewRecipeSection with the new section DTO list
        int sectionsRecords = 0;
        List<SectionDTO> addedSections = [];
        if (sectionsList.Count > 1)
        {
            (sectionsRecords, addedSections) = await _sectionsRepository.AddNewRecipeSectionsAsync(sectionsList, newRecipeId);
        }

        // build ingredient list DTO
        var ingList = (newRecipeContent["ingredients"]?.ToObject<List<IngredientListDTO>>()) ?? throw new InvalidOperationException("Ingredients section missing or invalid");
        
        // call ingListRepository.AddNewRecipeIng with the new ingredient list DTO
        int ingAdded = _ingListRepository.AddNewRecipeIng(ingList, newRecipeId, addedSections);

        // build instruction list DTO
        var instructions = (newRecipeContent["instructions"]?.ToObject<List<InstructionDTO>>()) ?? throw new InvalidOperationException("Instructions section missing or invalid");
        
        // call instructionsRepository.AddNewRecipeIns with the new instruction DTO
        int insAdded = _instructionsRepository.AddNewRecipeIns(instructions, newRecipeId, addedSections);

        // build recipe tags DTO
        var recipeTags = (newRecipeContent["recipeTags"]?.ToObject<RecipeTagDTO>()) ?? throw new InvalidOperationException("RecipeTags section missing or invalid");

        // call tagsRepository.AddNewRecipeTags with the new tags DTO
        int tagsAdded = _tagsRepository.AddNewRecipeTags(recipeTags, newRecipeId);

        var result = new OperationResult<int>();

        // check if all entries were added successfully with sections
        if (sectionsList.Count > 0 & sectionsRecords > 0 & ingAdded > 0 & insAdded > 0 & tagsAdded > 0)
        {
            result.Success = true;
            result.Data = newRecipeId;
            result.Message = "Recipe added successfully";
        }
        // check if all entries were added successfully without sections
        else if (sectionsList.Count == 0 & ingAdded > 0 & insAdded > 0 & tagsAdded > 0)
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