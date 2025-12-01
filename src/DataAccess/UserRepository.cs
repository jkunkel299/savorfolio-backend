/* Data access layer to User and UserRecipe entities */

using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.DataAccess;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<int> AddUserAsync(
        UserRegisterDTO userRegister,
        string password_hash,
        string password_salt
    )
    {
        // generate new user record to be input into User table
        var newUser = new User
        {
            // Username = userRegister.Username,
            Email = userRegister.Email,
            PasswordHash = password_hash,
            PasswordSalt = password_salt,
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return newUser.Id;
    }

    public async Task<int> AddUserRecipeAsync(int userId, int recipeId)
    {
        var newUserRecipe = new UserRecipe { UserId = userId, RecipeId = recipeId };
        _context.UserRecipes.Add(newUserRecipe);
        int record = await _context.SaveChangesAsync();
        return record;
    }
}
