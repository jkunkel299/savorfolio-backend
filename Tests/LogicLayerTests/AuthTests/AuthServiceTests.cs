using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Models.DTOs;

namespace Tests.LogicLayerTests.AuthTests;

public class AuthServiceTests
{
    private static AuthService authService;

    static AuthServiceTests()
    {
        authService = new();
    }

    // test for CreatePasswordHash
    [Fact]
    public void CreatePasswordHash_Test()
    {
        var password = "Savorfolio2025!";

        // call CreatePasswordHash on the same password twice
        var (hash1, salt1) = authService.CreatePasswordHash(password);
        var (hash2, salt2) = authService.CreatePasswordHash(password);

        // assert that the password hash and salt are not null
        Assert.False(string.IsNullOrEmpty(hash1));
        Assert.False(string.IsNullOrEmpty(salt1));

        // assert that the password hash and salt are of type Base64
        Assert.True(IsBase64(hash1));
        Assert.True(IsBase64(salt1));

        // assert that result 1 hash and salt are not equal to result 2 salt and hash
        // salt/key is new each time
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2);
    }

    // test for VerifyPassword returning true
    [Fact]
    public void VerifyPassword_ReturnsTrue()
    {
        // initialize variables
        string password = "Savorfolio2025!";
        string passwordHash = "An2XrT7LQ/RE92lOz6SbNiDNB4avzPc2zU1Aud8OyEA=";
        string passwordSalt =
            "JJQjxso8QQVpaeKrZET7hjko35TgQ0xLPEXYWW+YlRcaboZvnpeAp1N8q7XWAb/XIOmRs8IeJ8Nn8MIwbNE6aQ==";

        // call VerifyPassword
        var result = authService.VerifyPassword(password, passwordHash, passwordSalt);

        // Assert result is True
        Assert.True(result);
    }

    // test for VerifyPassword returning false
    [Fact]
    public void VerifyPassword_ReturnsFalse()
    {
        // initialize variables
        string password = "NotTheRightPassword";
        string passwordHash = "An2XrT7LQ/RE92lOz6SbNiDNB4avzPc2zU1Aud8OyEA=";
        string passwordSalt =
            "JJQjxso8QQVpaeKrZET7hjko35TgQ0xLPEXYWW+YlRcaboZvnpeAp1N8q7XWAb/XIOmRs8IeJ8Nn8MIwbNE6aQ==";

        // call VerifyPassword
        var result = authService.VerifyPassword(password, passwordHash, passwordSalt);

        // Assert result is False
        Assert.False(result);
    }

    private static bool IsBase64(string value)
    {
        Span<byte> buffer = new(new byte[value.Length]);
        return Convert.TryFromBase64String(value, buffer, out _);
    }
}
