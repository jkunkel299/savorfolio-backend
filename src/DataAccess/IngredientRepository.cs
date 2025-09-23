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
        var query = from i in _context.IngredientVariants
            join t in _context.IngredientTypes on i.TypeId equals t.Id // explicit join to avoid null deference
            select new IngredientVariantDTO
            {
                Id = i.Id,
                Name = i.Name,
                TypeId = i.TypeId,
                IngredientCategory = t.Name
            };
        // var query = _context.IngredientVariants
        //     .Include(t => t.Type)
        //     .Select(i => new IngredientVariantDTO
        //     {
        //         Id = i.Id,
        //         Name = i.Name,
        //         TypeId = i.TypeId,
        //         IngredientCategory = i.Type.Name
        //     });

        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            // fallback for testing
            query = query.Where(i => i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            query = query.Where(i => EF.Functions.ILike(i.Name, $"%{searchTerm}%"));
        }
        ;  

        return await query
            .OrderBy(i => i.Name)
            .Take(10)
            .ToListAsync();
    }
}