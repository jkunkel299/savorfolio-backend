using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Fixtures;
using Newtonsoft.Json;
using savorfolio_backend.Models.DTOs;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class AuthTests(DatabaseFixture databaseFixture, TestServerFixture testServerFixture)
    : IClassFixture<DatabaseFixture>,
        IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly TestServerFixture _testServerFixture = testServerFixture;
    private HttpClient _client = null!;

    #region Register
    // integration test for registering a new user successfully
    [Fact]
    public async Task RegisterUserIntegration_Success()
    {
        // create a new instance of HTTP Client using the unauthenticated client
        _client = new HttpClient();
        _client = _testServerFixture.UnauthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize expected user ID
        int newUserId = 2;

        // initialize userRegisterDTO
        UserRegisterDTO user = new()
        {
            Email = "testUser@savorfolio.com",
            Password = "testPassword1234",
        };

        // initialize the expected response message
        string expectedMessage = $"User {newUserId} registered successfully!";

        // call MapRegister API with the user to register
        var response = await _client.PostAsJsonAsync("/api/auth/register", user);
        // get the response content/message
        var rawMessage = await response.Content.ReadAsStringAsync();
        string? actualMessage = JsonConvert.DeserializeObject<string>(rawMessage);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(actualMessage);

        // Assert equal
        Assert.Equal(expectedMessage, actualMessage);
    }

    // integration test for registering a new user unsuccessfully - user already exists with the email provided
    [Fact]
    public async Task RegisterUserIntegration_Failure()
    {
        // create a new instance of HTTP Client using the unauthenticated client
        _client = new HttpClient();
        _client = _testServerFixture.UnauthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize userRegisterDTO
        UserRegisterDTO user = new()
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "testPassword1234",
        };

        // initialize the expected response message
        string expectedMessage = "A user with that Email already exists.";

        // call MapRegister API with the user to register
        var response = await _client.PostAsJsonAsync("/api/auth/register", user);
        // get the response content/message
        var rawMessage = await response.Content.ReadAsStringAsync();
        string? actualMessage = JsonConvert.DeserializeObject<string>(rawMessage);

        // Assert BadRequest
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Assert results not null
        Assert.NotNull(actualMessage);

        // Assert equal
        Assert.Equal(expectedMessage, actualMessage);
    }
    #endregion

    #region Login
    // integration test for logging in an existing user successfully
    [Fact]
    public async Task LoginUserIntegration_Success()
    {
        // create a new instance of HTTP Client using the unauthenticated client
        _client = new HttpClient();
        _client = _testServerFixture.UnauthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize userLoginDTO
        UserLoginDTO user = new()
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };

        // call MapLogin API with the user to log in
        var response = await _client.PostAsJsonAsync("/api/auth/login", user);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // integration test for logging in an existing user unsuccessfully due to incorrect password
    [Fact]
    public async Task LoginUserIntegration_PasswordFailure()
    {
        // create a new instance of HTTP Client using the unauthenticated client
        _client = new HttpClient();
        _client = _testServerFixture.UnauthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize userLoginDTO
        UserLoginDTO user = new()
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "NotTheRightPassword",
        };

        // call MapLogin API with the user to log in
        var response = await _client.PostAsJsonAsync("/api/auth/login", user);

        // Assert status code 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // integration test for logging in an existing user unsuccessfully due to incorrect email
    [Fact]
    public async Task LoginUserIntegration_EmailFailure()
    {
        // create a new instance of HTTP Client using the unauthenticated client
        _client = new HttpClient();
        _client = _testServerFixture.UnauthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize userLoginDTO
        UserLoginDTO user = new()
        {
            Email = "notAUser@savorfolio.com",
            Password = "NotTheRightPassword",
        };

        // call MapLogin API with the user to log in
        var response = await _client.PostAsJsonAsync("/api/auth/login", user);

        // Assert status code 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    #endregion

    #region Logout
    // integration test for logging out an existing user
    [Fact]
    public async Task LogoutUserIntegration()
    {
        // create a new instance of HTTP Client using pre-authenticated client
        _client = new HttpClient();
        _client = _testServerFixture.AuthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize the expected response message
        string expectedMessage = "logged out";

        // call MapLogout API
        var response = await _client.PostAsync("/api/auth/logout", content: null);

        // get the response content/message
        var rawMessage = await response.Content.ReadAsStringAsync();
        string? actualMessage = JsonConvert.DeserializeObject<string>(rawMessage);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(actualMessage);

        // Assert equal
        Assert.Equal(expectedMessage, actualMessage);
    }
    #endregion

    #region Fetch User
    // integration test for fetch user endpoint success
    [Fact]
    public async Task FetchUserIntegration_Success()
    {
        // create a new instance of HTTP Client using pre-authenticated client
        _client = new HttpClient();
        _client = _testServerFixture.AuthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize expected data
        var expected = """{"id":"1","email":"capstoneUser@savorfolio.com"}""";

        // call MapFetchUser API endpoint
        var response = await _client.GetAsync("/api/auth/me");

        // get content from response
        var content = await response.Content.ReadAsStringAsync();

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(content);

        // Assert equal
        Assert.Equal(expected, content);
    }

    // integration test for fetch user endpoint unauthorized
    [Fact]
    public async Task FetchUserIntegration_Unauthorized()
    {
        // create a new instance of HTTP Client using the unauthenticated client
        _client = new HttpClient();
        _client = _testServerFixture.UnauthenticatedClient;

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // call MapFetchUser API endpoint
        var response = await _client.GetAsync("/api/auth/me");

        // Assert status code 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    #endregion
}
