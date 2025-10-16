using IntegrationTests.Fixtures;
using savorfolio_backend.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using savorfolio_backend.Models.DTOs;
using System.Net;

namespace IntegrationTests.TestFiles;

// [Collection("Database Collection")]
[Collection("Integration Test Server")]
public class UnitSearchIntegrationTests(DatabaseFixture databaseFixture, TestServerFixture testServerFixture) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.HttpClient;

    [Fact]
    public async Task UnitSearchDatabase()
    {
        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // initialize search term
        string searchTerm = "ta";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        [
            {
                "Id": 37,
                "Name": "to taste",
                "Abbreviation": "to taste"
            },
            {
                "Id": 2,
                "Name": "Tablespoon",
                "Abbreviation": "tbsp"
            }
        ]
        """;
        JToken expectedToken = JToken.Parse(expectedJson);

        // instantiate repository
        var unitRepository = new UnitsRepository(_databaseFixture.Context);

        // Call SearchByNameAsync with search term
        var result = await unitRepository.SearchUnitTableAsync(searchTerm);

        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }



    [Fact]
    public async Task UnitSearchFullIntegration()
    {
        // initialize search term
        string searchTerm = "ta";
        string url = $"/api/units?term={searchTerm}";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        [
            {
                "Id": 37,
                "Name": "to taste",
                "Abbreviation": "to taste"
            },
            {
                "Id": 2,
                "Name": "Tablespoon",
                "Abbreviation": "tbsp"
            }
        ]
        """;
        JToken expectedToken = JToken.Parse(expectedJson);

        var response = await _client.GetAsync(url);
        var units = await response.Content.ReadFromJsonAsync<List<UnitDTO>>();
        // var orderedResult = ingredients!.OrderBy(i => i.Name).ToList();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(units);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(units);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}