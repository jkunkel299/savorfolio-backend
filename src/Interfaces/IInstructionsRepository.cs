using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IInstructionsRepository
{
    int AddNewRecipeIns(
        List<InstructionDTO> instructionsData,
        int recipeId,
        List<SectionDTO>? sectionsData
    );
    Task<List<InstructionDTO>> GetInstructionsByRecipeAsync(int recipeId);
}
