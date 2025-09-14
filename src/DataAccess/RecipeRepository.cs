using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Models;

namespace savorfolio_backend.DataAccess;

public class RecipeRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<Recipe>> ReturnAllRecipes()
    {
        var result = await _context.Recipes
            .OrderBy(i => i.Name)
            .ToListAsync();

        return result;
    }
}