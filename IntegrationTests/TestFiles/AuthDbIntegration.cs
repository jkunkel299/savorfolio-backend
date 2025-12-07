using IntegrationTests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Models.DTOs;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class AuthDbIntegration(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;

    [Fact]
    public async Task GetUserByEmail_DbIntegration()
    {
        // instantiate repository
        var userRepository = new UserRepository(_databaseFixture.Context);

        // initialize user email
        string userEmail = "capstoneUser@savorfolio.com";
        // initialize expected user
        var expectedUser = new UserDTO
        {
            Id = 1,
            Email = "capstoneUser@savorfolio.com",
            PasswordHash = "An2XrT7LQ/RE92lOz6SbNiDNB4avzPc2zU1Aud8OyEA=",
            PasswordSalt =
                "JJQjxso8QQVpaeKrZET7hjko35TgQ0xLPEXYWW+YlRcaboZvnpeAp1N8q7XWAb/XIOmRs8IeJ8Nn8MIwbNE6aQ==",
        };

        // convert to JSON
        string expectedJson = JsonConvert.SerializeObject(expectedUser);
        JToken expectedToken = JToken.Parse(expectedJson);

        // call GetByEmailAsync
        var result = await userRepository.GetByEmailAsync(userEmail);

        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public async Task AddUser_DbIntegration()
    {
        // instantiate repository
        var userRepository = new UserRepository(_databaseFixture.Context);

        // initialize the user register DTO
        var userRegister = new UserRegisterDTO
        {
            Email = "newUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };

        // initialize expected ID
        var expectedId = 2;

        // initialize password hash
        var password_hash = "CZyNf3gyy4xbn5mGtcm/1uUMPOpgm7mPyjk8iUKlpqM=";
        // initialize password salt
        var password_salt =
            "HNuk8465dc1Gzj+5UlIIGcI/LLEwZ2Hvh5snSkAnLSWWVmEbNo9BRCgXx1mRVb5VEEs9vktYGZoo2zgu4tvq1Q==";

        // call AddUserAsync
        var resultId = await userRepository.AddUserAsync(
            userRegister,
            password_hash,
            password_salt
        );

        // assert the expected user ID is equal to the added user ID
        Assert.Equal(expectedId, resultId);
    }
}
