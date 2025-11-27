/* Business logic layer for managing JWT-based authentication, encryption of password data */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;

namespace savorfolio_backend.LogicLayer;

public class AuthService(JwtSettings jwtSettings) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings;

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

    // Generate JWT token
    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim("id", user.Id.ToString()),
            // new Claim("username", user.Username),
            new Claim("email", user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(18),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
