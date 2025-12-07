using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using savorfolio_backend.Models.DTOs;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class IngredientSearchIntegrationTests(
    DatabaseFixture databaseFixture,
    TestServerFixture testServerFixture
) : IClassFixture<DatabaseFixture>, IClassFixture<TestServerFixture>
{
    private readonly DatabaseFixture _databaseFixture = databaseFixture;
    private readonly HttpClient _client = testServerFixture.AuthenticatedClient;

    [Fact]
    public async Task IngredientSearchDatabase()
    {
        // initialize search term
        string searchTerm = "chicken";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
            [
                {
                    "Id": 143,
                    "Name": "chicken",
                    "PluralName": null,
                    "TypeId": 7,
                    "IngredientCategory": "Protein"
                },
                {
                    "Id": 20,
                    "Name": "chicken stock",
                    "PluralName": null,
                    "TypeId": 4,
                    "IngredientCategory": "Broth & Stock"
                },
                {
                    "Id": 144,
                    "Name": "chicken breast",
                    "PluralName": null,
                    "TypeId": 7,
                    "IngredientCategory": "Protein"
                },
                {
                    "Id": 145,
                    "Name": "chicken thigh",
                    "PluralName": null,
                    "TypeId": 7,
                    "IngredientCategory": "Protein"
                },
                {
                    "Id": 251,
                    "Name": "chicken broth",
                    "PluralName": null,
                    "TypeId": 4,
                    "IngredientCategory": "Broth & Stock"
                },
                {
                    "Id": 287,
                    "Name": "chicken stock base",
                    "PluralName": null,
                    "TypeId": 4,
                    "IngredientCategory": "Broth & Stock"
                }
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
                {
                    "Id": 143,
                    "Name": "chicken",
                    "PluralName": null,
                    "TypeId": 7,
                    "IngredientCategory": "Protein"
                },
                {
                    "Id": 20,
                    "Name": "chicken stock",
                    "PluralName": null,
                    "TypeId": 4,
                    "IngredientCategory": "Broth & Stock"
                },
                {
                    "Id": 144,
                    "Name": "chicken breast",
                    "PluralName": null,
                    "TypeId": 7,
                    "IngredientCategory": "Protein"
                },
                {
                    "Id": 145,
                    "Name": "chicken thigh",
                    "PluralName": null,
                    "TypeId": 7,
                    "IngredientCategory": "Protein"
                },
                {
                    "Id": 251,
                    "Name": "chicken broth",
                    "PluralName": null,
                    "TypeId": 4,
                    "IngredientCategory": "Broth & Stock"
                },
                {
                    "Id": 287,
                    "Name": "chicken stock base",
                    "PluralName": null,
                    "TypeId": 4,
                    "IngredientCategory": "Broth & Stock"
                }
            ]
            """;
        JToken expectedToken = JToken.Parse(expectedJson);

        var response = await _client.GetAsync(url);
        var ingredients = await response.Content.ReadFromJsonAsync<List<IngredientVariantDTO>>();
        var result = ingredients!.ToList();
        // var orderedResult = ingredients!.OrderBy(i => i.Name).ToList();

        // Convert result to JSON token
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(ingredients);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}
