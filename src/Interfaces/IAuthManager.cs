using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IAuthManager
{
    Task<OperationResult<IResult>> RegisterUserAsync(UserRegisterDTO userRegister);
    Task<OperationResult<IResult>> LoginUserAsync(UserLoginDTO userLogin);
}
