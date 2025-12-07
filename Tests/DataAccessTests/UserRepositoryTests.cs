using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Data;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;
using Tests.Fixtures;
using Tests.Helpers;

namespace Tests.DataAccessTests;

[Collection("SQLite test database Collection")]
public class UserRepositoryTests(SqliteDbFixture sqliteDbFixture) : IClassFixture<SqliteDbFixture>
{
    private readonly UserRepository _repository = new(sqliteDbFixture.Context);
    private readonly RecipeRepository _recipeRepository = new(sqliteDbFixture.Context);
    private static readonly User _expectedUser;
    private static readonly JObject _expectedAddRecipe;

    static UserRepositoryTests()
    {
        _expectedUser = new()
        {
            Id = 1,
            Email = "capstoneUser@savorfolio.com",
            PasswordHash = "An2XrT7LQ/RE92lOz6SbNiDNB4avzPc2zU1Aud8OyEA=",
            PasswordSalt =
                "JJQjxso8QQVpaeKrZET7hjko35TgQ0xLPEXYWW+YlRcaboZvnpeAp1N8q7XWAb/XIOmRs8IeJ8Nn8MIwbNE6aQ==",
        };
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }

    [Fact]
    public async Task GetUserByEmailTest()
    {
        // initialize the test string Email
        var testEmail = "capstoneUser@savorfolio.com";

        // initialize expected result
        var expectedUser = _expectedUser;

        // call GetByEmailAsync with testEmail
        var result = await _repository.GetByEmailAsync(testEmail);

        // assert the result is not null
        Assert.NotNull(result);

        // Assert the expected and actual emails are equal
        Assert.Equal(expectedUser.Email, result.Email);

        // Assert the expected and actual password hash are equal
        Assert.Equal(expectedUser.PasswordHash, result.PasswordHash);

        // Assert the expected and actual password salt are equal
        Assert.Equal(expectedUser.PasswordSalt, result.PasswordSalt);
    }

    [Fact]
    public async Task AddUserTest()
    {
        // initialize user register DTO
        UserRegisterDTO testUser = new()
        {
            Email = "testUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };
        // initialize password hash
        var password_hash = "CZyNf3gyy4xbn5mGtcm/1uUMPOpgm7mPyjk8iUKlpqM=";
        // initialize password salt
        var password_salt =
            "HNuk8465dc1Gzj+5UlIIGcI/LLEwZ2Hvh5snSkAnLSWWVmEbNo9BRCgXx1mRVb5VEEs9vktYGZoo2zgu4tvq1Q==";

        // initialize new user ID
        int expectedId = 2;

        // Call AddUserAsync with the initialized inputs
        var resultId = await _repository.AddUserAsync(testUser, password_hash, password_salt);

        // assert the expected user ID is equal to the added user ID
        Assert.Equal(expectedId, resultId);
    }

    [Fact]
    public async Task AddUserRecipeTest()
    {
        // initialize the recipe DTO to add to the database
        var addRecipeDTO = _expectedAddRecipe["recipeSummary"]?.ToObject<RecipeDTO>();
        // initialize the user to add to the table
        var userId =
            (_expectedAddRecipe["userId"]?.ToObject<int>())
            ?? throw new InvalidOperationException("UserId section missing or invalid");

        // call AddNewRecipeAsync with the DTO -- this is necessary to avoid foreign key violations
        var recipeId = await _recipeRepository.AddNewRecipeAsync(addRecipeDTO!);

        // call AddUserRecipeAsync with initialized user and recipe IDs
        _ = await _repository.AddUserRecipeAsync(userId, recipeId);

        // get the added user-recipe record from the table
        var addedRecord = await sqliteDbFixture.Context.UserRecipes.FirstOrDefaultAsync(ur =>
            ur.UserId == userId && ur.RecipeId == recipeId
        );

        // ensure the user-recipe record exists
        Assert.NotNull(addedRecord);
    }
}
