namespace savorfolio_backend.Models.DTOs;

public partial class UserDTO
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    // public virtual ICollection<CustomTagDTO> CustomTags { get; set; } = [];

    // public virtual ICollection<NoteDTO> Notes { get; set; } = [];
}
