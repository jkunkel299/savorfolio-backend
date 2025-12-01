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

        string email = userRegister.Email;
        string password = userRegister.Password;

        var existingUserEmail = await _userRepository.GetByEmailAsync(email);

        if (existingUserEmail != null)
        {
            result.Success = false;
            result.Message = "A user with that Email already exists.";
            // result.Data = Results.Ok();
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

    public async Task<OperationResult<IResult>> LoginUserAsync(
        UserLoginDTO userLogin,
        HttpContext context
    )
    {
        var user = await _userRepository.GetByEmailAsync(userLogin.Email);

        if (
            user == null
            || !_authService.VerifyPassword(
                userLogin.Password,
                user.PasswordHash,
                user.PasswordSalt
            )
        )
        {
            return new OperationResult<IResult>
            {
                Success = false,
                Message = "Invalid login",
                Data = Results.Unauthorized(),
            };
        }

        await _authService.GenerateCookies(user, context);

        return new OperationResult<IResult>
        {
            Success = true,
            Message = "Logged in",
            Data = Results.Ok(),
        };
    }
}
