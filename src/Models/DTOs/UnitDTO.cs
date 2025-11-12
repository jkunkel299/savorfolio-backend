using savorfolio_backend.Interfaces;

namespace savorfolio_backend.Models.DTOs;

public class UnitDTO : IDTOInterface
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public string? PluralName { get; set; }
}
