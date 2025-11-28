using savorfolio_backend.Models;

namespace savorfolio_backend.Interfaces;

public interface IAuthService
{
    (string hash, string salt) CreatePasswordHash(string password);
    bool VerifyPassword(string password, string storedHash, string storedSalt);
    string GenerateJwtToken(User user);
}
