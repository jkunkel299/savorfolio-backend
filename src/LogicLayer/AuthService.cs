/* Business logic layer for managing cookie-based authentication, encryption of password data */

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class AuthService() : IAuthService
{
    // Create password hash and salt as Base64 strings
    public (string hash, string salt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA256();
        var saltBytes = hmac.Key;
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        string hash = Convert.ToBase64String(hashBytes);
        string salt = Convert.ToBase64String(saltBytes);

        return (hash, salt);
    }

    // Verify password using Base64-decode hash/salt
    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var hashBytes = Convert.FromBase64String(storedHash);
        var saltBytes = Convert.FromBase64String(storedSalt);

        using var hmac = new HMACSHA256(saltBytes);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(hashBytes);
    }

    // Generate Cookies
    public async Task GenerateCookies(UserDTO user, HttpContext context)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
        };
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(18),
            }
        );
    }
}
