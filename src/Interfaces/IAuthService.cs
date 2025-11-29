using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IAuthService
{
    (string hash, string salt) CreatePasswordHash(string password);
    bool VerifyPassword(string password, string storedHash, string storedSalt);
    Task GenerateCookies(User user, HttpContext context);
}
