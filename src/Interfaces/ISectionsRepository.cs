using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface ISectionsRepository
{
    Task<(int records, List<SectionDTO> addedSections)> AddNewRecipeSectionsAsync(List<SectionDTO> sectionsData, int recipeId);
    Task<List<SectionDTO>> GetSectionsByRecipeAsync(int recipeId);
}