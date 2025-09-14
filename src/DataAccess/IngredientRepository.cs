using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;

namespace savorfolio_backend.DataAccess;

public class IngredientRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<IngredientVariant>> SearchByNameAsync(string searchTerm)
    {
        var result = await _context.IngredientVariants
            .Where(i => EF.Functions.ILike(i.Name, $"%{searchTerm}%"))
            .OrderBy(i => i.Name)
            .Take(10)
            .ToListAsync();

        return result;
    }
}