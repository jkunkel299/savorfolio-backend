using Moq;
using savorfolio_backend.Models.DTOs;

namespace Tests.TestData;

public class LoginAuthFailData
{
    public static TheoryData<UserDTO?, bool> LoginFailTestCases() =>
        new()
        {
            { null, true },
            { userDTO, false },
            { null, false },
        };

    private static readonly UserDTO userDTO = new()
    {
        Id = 1,
        Email = "capstoneUser@savorfolio.com",
        PasswordHash = "fakehash",
        PasswordSalt = "fakesalt",
    };
}
