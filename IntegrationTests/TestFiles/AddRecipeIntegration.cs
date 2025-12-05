using System.Net;
using System.Text;
using IntegrationTests.Fixtures;
using Newtonsoft.Json.Linq;
using Tests.Helpers;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class AddRecipeFullTests(
    DatabaseFixture databaseFixture,
    TestServerFixture testServerFixture
) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.AuthenticatedClient;
    private static readonly JObject _expectedAddRecipe;
    private static readonly JObject _expectedAddRecipeSections;

    static AddRecipeFullTests()
    {
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");
        string addRecipeSectionsFilePath = TestFileHelper.GetProjectPath(
            "ExpectedData/AddRecipeSections.json"
        );
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
        _expectedAddRecipeSections = JObject.Parse(File.ReadAllText(addRecipeSectionsFilePath));
    }

    [Fact]
    public async Task AddRecipesTest_NoSections()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize expected recipe ID
        int expectedId = 4;
        // initialize the expected response message
        string expectedMessage = $"\"Recipe ID {expectedId} added successfully\"";

        // initialize JSON body
        string jsonBody = _expectedAddRecipe.ToString();

        // Prepare HTTP content with correct JSON headers
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Call MapManualRecipe API with the recipe to add
        var response = await _client.PostAsync("/api/recipes/add/manual", content);
        // get the response content/message
        var actualMessage = await response.Content.ReadAsStringAsync();

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(actualMessage);

        // Assert equal
        Assert.Equal(expectedMessage, actualMessage);
    }

    [Fact]
    public async Task AddRecipesTest_Sections()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize expected recipe ID
        int expectedId = 5;
        // initialize the expected response message
        string expectedMessage = $"\"Recipe ID {expectedId} added successfully\"";

        // initialize JSON body
        string jsonBody = _expectedAddRecipeSections.ToString();

        // Prepare HTTP content with correct JSON headers
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Call MapManualRecipe API with the recipe to add
        var response = await _client.PostAsync("/api/recipes/add/manual", content);
        // get the response content/message
        var actualMessage = await response.Content.ReadAsStringAsync();

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(actualMessage);

        // Assert equal
        Assert.Equal(expectedMessage, actualMessage);
    }
}
