/* Data access layer to Ingredient_Variants entity */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class IngredientRepository(AppDbContext context) : IIngredientRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<IngredientVariantDTO>> SearchByNameAsync(string searchTerm)
    {
        searchTerm = searchTerm.ToLower();
        var query = _context.IngredientVariants.Include(i => i.Type).AsQueryable();

        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            // fallback for testing
            query = query
                .Where(i => i.Name.Contains(searchTerm))
                .OrderByDescending(i =>
                    i.Name == searchTerm ? 3
                    : i.Name.StartsWith(searchTerm + ",") || i.Name.StartsWith(searchTerm + " ") ? 2
                    : i.Name.Contains(searchTerm) ? 1
                    : 0
                );
        }
        else
        {
            query = query
                .Where(i =>
                    EF.Functions.ILike(i.Name, $"%{searchTerm}%")
                    || EF.Functions.ILike(i.PluralName!, $"%{searchTerm}%")
                )
                .OrderByDescending(i =>
                    i.Name == searchTerm ? 3
                    : EF.Functions.ILike(i.Name, $"{searchTerm},%")
                    || EF.Functions.ILike(i.Name, $"{searchTerm} %")
                        ? 2
                    : EF.Functions.ILike(i.Name, $"%{searchTerm}%") ? 1
                    : 0
                );
        }
        ;

        return await query
            .Select(i => new IngredientVariantDTO
            {
                Id = i.Id,
                Name = i.Name,
                TypeId = i.TypeId,
                IngredientCategory = i.Type!.Name,
                PluralName = i.PluralName!,
            })
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<string>> IngredientSearchReturnStringAsync(string? searchTerm)
    {
        List<string> ingNames = [];
        if (searchTerm == null || searchTerm == "")
            return ["none"];
        var ingMatch = await SearchByNameAsync(searchTerm);
        if (ingMatch.Count == 0)
            return ["none"];
        foreach (var ing in ingMatch)
        {
            ingNames.Add(ing.Name);
        }
        return ingNames;
    }
}
