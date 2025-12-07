using Microsoft.AspNetCore.Http;
using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer;
using savorfolio_backend.Models;
using savorfolio_backend.Models.DTOs;
using Tests.TestData;

namespace Tests.LogicLayerTests.AuthTests;

public class AuthManagerTests
{
    //mock user repository interface
    private readonly Mock<IUserRepository> mockUserRepo;

    // mock auth service interface
    private readonly Mock<IAuthService> mockAuthService;

    // mock AuthManager
    private readonly AuthManager authManager;

    // mock HttpContext
    private readonly DefaultHttpContext httpContext;

    // set default user DTO
    private readonly UserDTO userDTO;

    public AuthManagerTests()
    {
        mockUserRepo = new Mock<IUserRepository>();
        mockAuthService = new Mock<IAuthService>();
        authManager = new AuthManager(mockUserRepo.Object, mockAuthService.Object);
        httpContext = new DefaultHttpContext();
        userDTO = new UserDTO
        {
            Id = 1,
            Email = "capstoneUser@savorfolio.com",
            PasswordHash = Convert.ToBase64String(new byte[32]),
            PasswordSalt = Convert.ToBase64String(new byte[32]),
        };
    }

    #region Register
    // test for register calling dependent functions
    [Fact]
    public async Task RegisterUserAsync_CallsDependent()
    {
        // initialize user to register
        var userRegister = new UserRegisterDTO
        {
            Email = "testNewUser@savorfolio.com",
            Password = "testPassword1",
        };

        // set the return values for dependent functions
        mockUserRepo
            .Setup(u => u.GetByEmailAsync(userRegister.Email))
            .ReturnsAsync((UserDTO?)null);
        mockAuthService
            .Setup(u => u.CreatePasswordHash(userRegister.Password))
            .Returns((It.IsAny<string>(), It.IsAny<string>()));
        mockUserRepo
            .Setup(u => u.AddUserAsync(userRegister, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<int>());

        // call RegisterUserAsync
        _ = await authManager.RegisterUserAsync(userRegister);

        // assert GetByEmailAsync called once
        mockUserRepo.Verify(d => d.GetByEmailAsync(userRegister.Email), Times.AtMostOnce);
        // assert CreatePasswordHash called once
        mockAuthService.Verify(d => d.CreatePasswordHash(userRegister.Password), Times.AtMostOnce);
        // assert AddUserAsync called once
        mockUserRepo.Verify(
            d => d.AddUserAsync(userRegister, It.IsAny<string>(), It.IsAny<string>()),
            Times.AtMostOnce
        );
    }

    // test for register returning success
    [Fact]
    public async Task RegisterUserAsync_ReturnsSuccess()
    {
        // initialize user to register
        var userRegister = new UserRegisterDTO
        {
            Email = "testNewUser@savorfolio.com",
            Password = "testPassword1",
        };
        // initialize expected return.message
        string expectedMessage = "User 2 registered successfully!";

        // set the return values for dependent functions
        mockUserRepo
            .Setup(u => u.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDTO?)null);
        mockAuthService
            .Setup(u => u.CreatePasswordHash(It.IsAny<string>()))
            .Returns((It.IsAny<string>(), It.IsAny<string>()));
        mockUserRepo
            .Setup(u =>
                u.AddUserAsync(It.IsAny<UserRegisterDTO>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .ReturnsAsync(2);

        // call RegisterUserAsync
        var result = await authManager.RegisterUserAsync(userRegister);

        // assert result.success is true
        Assert.True(result.Success);

        // assert result.message is expected
        Assert.Equal(expectedMessage, result.Message);
    }

    // test for register returning fail
    [Fact]
    public async Task RegisterUserAsync_ReturnsFailure()
    {
        // initialize user to register
        var userRegister = new UserRegisterDTO
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };
        // initialize expected return.message
        string expectedMessage = "A user with that Email already exists.";

        // set the return values for dependent functions
        mockUserRepo.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(userDTO);
        mockAuthService
            .Setup(u => u.CreatePasswordHash(It.IsAny<string>()))
            .Returns((It.IsAny<string>(), It.IsAny<string>()));
        mockUserRepo
            .Setup(u =>
                u.AddUserAsync(It.IsAny<UserRegisterDTO>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .ReturnsAsync(It.IsAny<int>());

        // call RegisterUserAsync
        var result = await authManager.RegisterUserAsync(userRegister);

        // assert result.success is false
        Assert.False(result.Success);

        // assert result.message is expected
        Assert.Equal(expectedMessage, result.Message);
    }
    #endregion

    #region Login
    // test for login calling dependent functions
    [Fact]
    public async Task LoginUserAsync_CallsDependentFunctions()
    {
        // initialize UserLoginDTO
        var userLogin = new UserLoginDTO
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };

        // set return values for dependent functions
        mockUserRepo.Setup(u => u.GetByEmailAsync(userLogin.Email)).ReturnsAsync(userDTO);
        mockAuthService
            .Setup(u =>
                u.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(true);

        // call LoginUserAsync
        _ = await authManager.LoginUserAsync(userLogin, httpContext);

        // assert GetByEmailAsync called once
        mockUserRepo.Verify(d => d.GetByEmailAsync(userLogin.Email), Times.AtMostOnce);
        // assert VerifyPassword called once
        mockAuthService.Verify(
            d => d.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.AtMostOnce
        );
        // assert GenerateCookies called once
        mockAuthService.Verify(d => d.GenerateCookies(userDTO, httpContext), Times.AtMostOnce);
    }

    // test for login returning success
    [Fact]
    public async Task LoginUserAsync_ReturnsSuccess()
    {
        // initialize UserLoginDTO
        var userLogin = new UserLoginDTO
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };
        // initialize expected return message
        string expectedMessage = "Logged in";

        // set return values for dependent functions
        mockUserRepo.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(userDTO);
        mockAuthService
            .Setup(u =>
                u.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(true);

        // call LoginUserAsync
        var result = await authManager.LoginUserAsync(userLogin, httpContext);

        // assert result.success is true
        Assert.True(result.Success);

        // assert result.message is expected
        Assert.Equal(expectedMessage, result.Message);

        // assert result.data is Results.Ok
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok>(result.Data);
    }

    // test for login returning fail
    [Theory]
    [MemberData(
        nameof(LoginAuthFailData.LoginFailTestCases),
        MemberType = typeof(LoginAuthFailData)
    )]
    public async Task LoginUserAsync_ReturnsFail(UserDTO? user, bool verified)
    {
        // initialize UserLoginDTO
        var userLogin = new UserLoginDTO
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "NotTheRightPassword",
        };
        // initialize expected return message
        string expectedMessage = "Invalid login";

        // set return values for dependent functions
        mockUserRepo.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        mockAuthService
            .Setup(u =>
                u.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(verified);

        // call LoginUserAsync
        var result = await authManager.LoginUserAsync(userLogin, httpContext);

        // assert result.success is false
        Assert.False(result.Success);

        // assert result.message is expected
        Assert.Equal(expectedMessage, result.Message);

        // assert result.data is Results.Unauthorized
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(
            result.Data
        );
    }
    #endregion
}
