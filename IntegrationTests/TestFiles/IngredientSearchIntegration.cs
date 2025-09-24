using IntegrationTests.Fixtures;
using savorfolio_backend.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using savorfolio_backend.Models.DTOs;
using System.Net;
using Azure;
using savorfolio_backend.Models;

namespace IntegrationTests.TestFiles;

// [Collection("Database Collection")]
[Collection("Integration Test Server")]
public class IngredientSearchIntegrationTests(DatabaseFixture databaseFixture, TestServerFixture testServerFixture) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.HttpClient;

    [Fact]
    public async Task IngredientSearchDatabase()
    {
        // initialize search term
        string searchTerm = "chicken";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        [
            {"Id": 143, "Name": "chicken", "TypeId": 7, "IngredientCategory": "Protein"},
            {"Id": 144, "Name": "chicken breast", "TypeId": 7, "IngredientCategory": "Protein"},
            {"Id": 251, "Name": "chicken broth", "TypeId": 4, "IngredientCategory": "Broth & Stock"},
            {"Id": 20, "Name": "chicken stock", "TypeId": 4, "IngredientCategory": "Broth & Stock"},
            {"Id": 145, "Name": "chicken thigh", "TypeId": 7, "IngredientCategory": "Protein"}
        ]
        """;
        JToken expectedToken = JToken.Parse(expectedJson);

        // Ensure connection to the Database
        Assert.False(string.IsNullOrEmpty(_databaseFixture.ConnectionString));

        // instantiate repository
        var ingredientRepository = new IngredientRepository(_databaseFixture.Context);

        // Call the SearchByNameAsync function, search term = "chicken"
        var results = await ingredientRepository.SearchByNameAsync(searchTerm);

        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(results);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public async Task TestFullIngredientSearch()
    {
        string searchTerm = "chicken";
        string url = $"/api/ingredients/search?term={searchTerm}";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        [
            {"Id": 143, "Name": "chicken", "TypeId": 7, "IngredientCategory": "Protein"},
            {"Id": 144, "Name": "chicken breast", "TypeId": 7, "IngredientCategory": "Protein"},
            {"Id": 251, "Name": "chicken broth", "TypeId": 4, "IngredientCategory": "Broth & Stock"},
            {"Id": 20, "Name": "chicken stock", "TypeId": 4, "IngredientCategory": "Broth & Stock"},
            {"Id": 145, "Name": "chicken thigh", "TypeId": 7, "IngredientCategory": "Protein"}
        ]
        """;
        var expected = JsonConvert.DeserializeObject<List<IngredientVariantDTO>>(expectedJson);

        var response = await _client.GetAsync(url);
        var actual = await response.Content.ReadFromJsonAsync<List<IngredientVariantDTO>>();

        // assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(response.Content);

        // assert result expected
        Assert.Equal(expected!.Count, actual!.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, actual[i].Id);
            Assert.Equal(expected[i].Name, actual[i].Name);
            Assert.Equal(expected[i].TypeId, actual[i].TypeId);
            Assert.Equal(expected[i].IngredientCategory, actual[i].IngredientCategory);
        }
    }
}