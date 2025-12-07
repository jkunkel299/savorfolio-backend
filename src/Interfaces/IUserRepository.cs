using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IUserRepository
{
    // Task<User?> GetByUsernameAsync(string username);
    Task<UserDTO?> GetByEmailAsync(string email);
    Task<int> AddUserAsync(
        UserRegisterDTO userRegister,
        string password_hash,
        string password_salt
    );
    Task<int> AddUserRecipeAsync(int userId, int recipeId);
}
