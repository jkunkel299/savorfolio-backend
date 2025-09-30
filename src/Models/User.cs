namespace savorfolio_backend.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<CustomTag> CustomTags { get; set; } = [];

    public virtual ICollection<Note> Notes { get; set; } = [];
}
