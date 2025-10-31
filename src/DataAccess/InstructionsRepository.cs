/* Data access layer to Instructions entity */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class InstructionsRepository(AppDbContext context) : IInstructionsRepository
{
    private readonly AppDbContext _context = context;

    public int AddNewRecipeIns(List<InstructionDTO> instructionsData, int recipeId, List<SectionDTO>? sectionsData)
    {
        // for each instruction in the list, create a new row in the Instructions table
        foreach (InstructionDTO instruction in instructionsData)
        {
            int? sectionId = null;

            if (sectionsData!.Count != 0 && !string.IsNullOrEmpty(instruction.SectionName))
            {
                var matchedSection = sectionsData
                    .FirstOrDefault(s =>
                        string.Equals(s.SectionName, instruction.SectionName, StringComparison.OrdinalIgnoreCase));

                sectionId = matchedSection?.Id;
            }
            var newInstruction = new Instruction
            {
                RecipeId = recipeId,
                SectionId = sectionId,
                StepNumber = instruction.StepNumber,
                InstructionText = instruction.InstructionText
            };
            Console.WriteLine($"Adding instruction step {newInstruction.StepNumber} with RecipeId {recipeId}");

            _context.Instructions.Add(newInstruction);
        }

        var result = _context.SaveChanges();
        return result;
    }

    public Task<List<InstructionDTO>> GetInstructionsByRecipeAsync(int recipeId)
    {
        var result = _context.Instructions
            .Where(i => i.RecipeId == recipeId)
            .Select(e => new InstructionDTO
            {
                Id = e.Id,
                RecipeId = e.RecipeId,
                StepNumber = e.StepNumber,
                InstructionText = e.InstructionText,
                SectionId = e.SectionId,
                SectionName = e.Section != null ? e.Section.SectionName : null
            })
            .OrderBy(e => e.StepNumber)
            .ToListAsync();

        return result;
    }
}