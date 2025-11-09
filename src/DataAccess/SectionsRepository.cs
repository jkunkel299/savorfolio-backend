/* Data access layer to Sections entity */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class SectionsRepository(AppDbContext context) : ISectionsRepository
{
    private readonly AppDbContext _context = context;

    public async Task<(int records, List<SectionDTO> addedSections)> AddNewRecipeSectionsAsync(List<SectionDTO> sectionsData, int recipeId)
    {
        // for each section in the list, create a new row in the Sections table
        foreach (SectionDTO section in sectionsData)
        {
            var newSection = new RecipeSection
            {
                RecipeId = recipeId,
                SectionName = section.SectionName,
                SortOrder = section.SortOrder
            };
            Console.WriteLine($"Adding section name {newSection.SectionName} with RecipeId {recipeId}");

            _context.RecipeSections.Add(newSection);
        }

        var result = _context.SaveChanges();

        var addedSections = await GetSectionsByRecipeAsync(recipeId);

        return (result, addedSections);
    }

    public Task<List<SectionDTO>> GetSectionsByRecipeAsync(int recipeId)
    {
        var result = _context.RecipeSections
            .Where(i => i.RecipeId == recipeId)
            .Select(e => new SectionDTO
            {
                Id = e.Id,
                RecipeId = e.RecipeId,
                SectionName = e.SectionName,
                SortOrder = e.SortOrder
            })
            .OrderBy(e => e.SortOrder)
            .ToListAsync();

        return result;
    }
}