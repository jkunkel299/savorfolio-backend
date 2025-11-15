/* Business logic layer for managing login and registration */

using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.LogicLayer;

public class AuthManager(IUserRepository userRepository, IAuthService authService) : IAuthManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuthService _authService = authService;

    public async Task<OperationResult<IResult>> RegisterUserAsync(UserRegisterDTO userRegister)
    {
        var result = new OperationResult<IResult>();

        string username = userRegister.Username;
        string email = userRegister.Email;
        string password = userRegister.Password;

        var existingUserUName = await _userRepository.GetByUsernameAsync(username);
        var existingUserEmail = await _userRepository.GetByUsernameAsync(email);

        if (existingUserUName != null)
        {
            result.Success = false;
            result.Message = "A user with that username already exists.";
            result.Data = Results.Ok();
            return result;
        }

        if (existingUserEmail != null)
        {
            result.Success = false;
            result.Message = "A user with that Email already exists.";
            result.Data = Results.Ok();
            return result;
        }

        (string password_hash, string password_salt) = _authService.CreatePasswordHash(password);

        var newUserId = await _userRepository.AddUserAsync(
            userRegister,
            password_hash,
            password_salt
        );

        result.Success = true;
        result.Message = $"User {newUserId} registered successfully!";

        return result;
    }

    public async Task<OperationResult<IResult>> LoginUserAsync(UserLoginDTO userLogin)
    {
        var result = new OperationResult<IResult>();
        var user = await _userRepository.GetByUsernameAsync(userLogin.Username);
        if (user == null)
        {
            result.Success = false;
            result.Message = "User is not authorized";
            result.Data = Results.Unauthorized();
            return result;
        }

        if (!_authService.VerifyPassword(userLogin.Password, user.PasswordHash, user.PasswordSalt))
        {
            result.Success = false;
            result.Message = "Password is not authorized";
            result.Data = Results.Unauthorized();
            return result;
        }

        var token = _authService.GenerateJwtToken(user!);

        result.Success = true;
        result.Data = Results.Ok(new { token });

        return result;
    }
}
