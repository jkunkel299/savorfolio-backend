/* Data access layer to Units entity */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class UnitsRepository(AppDbContext context) : IUnitsRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<UnitDTO>> SearchUnitTableAsync(string searchTerm)
    {
        var query = from u in _context.Units
                    select new UnitDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Abbreviation = u.Abbreviation,
                        PluralName = u.PluralName!
                    };

        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            // fallback for testing
            query = query.Where(u =>
                u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Abbreviation.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            )
            .OrderByDescending(u => 
                u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(u => 
                u.Abbreviation.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.Name, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.Abbreviation, $"%{searchTerm}%")
            )
                .OrderByDescending(e => EF.Functions.ILike(e.Name, $"%{searchTerm}%"))
                .ThenByDescending(e => EF.Functions.ILike(e.Abbreviation, $"%{searchTerm}%"));
        };

        return await query
            .Take(10)
            .ToListAsync();
    }
}