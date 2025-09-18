using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class IngredientRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<IngredientVariantDTO>> SearchByNameAsync(string searchTerm)
    {
        var result = _context.IngredientVariants
            .Include(t => t.Type)
            .Select(i => new IngredientVariantDTO
            {
                Id = i.Id,
                Name = i.Name,
                TypeId = i.TypeId,
                IngredientCategory = i.Type.Name
            })
            .Where(i => EF.Functions.ILike(i.Name, $"%{searchTerm}%"))
            .OrderBy(i => i.Name)
            .Take(10);

        return await result.ToListAsync();
    }

    // public async Task<List<IngredientVariant>> SearchByNameAsync(string searchTerm)
    // {
    //     var result = await _context.IngredientVariants
    //         .Where(i => EF.Functions.ILike(i.Name, $"%{searchTerm}%"))
    //         .OrderBy(i => i.Name)
    //         .Take(10)
    //         .ToListAsync();

    //     return result;
    // }
}