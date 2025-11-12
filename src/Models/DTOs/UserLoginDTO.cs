namespace savorfolio_backend.Models.DTOs;

public partial class UserLoginDTO
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
