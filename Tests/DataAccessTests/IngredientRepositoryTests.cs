using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using Tests.Fixtures;

namespace Tests.DataAccessTests;

[Collection("Unit test database Collection")]
public class IngredientRepositoryTests(UnitDbFixture unitDbFixture) : IClassFixture<UnitDbFixture>
{
    private readonly IngredientRepository repository = new(unitDbFixture.Context);

    [Fact]
    public async Task IngredientSearchTest()
    {
        // initialize search term
        string searchTerm = "chicken";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
            [
                {"Id": 143,"Name": "chicken","PluralName": null,"TypeId": 7,"IngredientCategory": "Protein"},
                {"Id": 20,"Name": "chicken stock","PluralName": null,"TypeId": 4,"IngredientCategory": "Broth & Stock"},
                {"Id": 144,"Name": "chicken breast","PluralName": null,"TypeId": 7,"IngredientCategory": "Protein"},
                {"Id": 145,"Name": "chicken thigh","PluralName": null,"TypeId": 7,"IngredientCategory": "Protein"},
                {"Id": 251,"Name": "chicken broth","PluralName": null,"TypeId": 4,"IngredientCategory": "Broth & Stock"},
                {"Id": 287,"Name": "chicken stock base","PluralName": null,"TypeId": 4,"IngredientCategory": "Broth & Stock"},
            ]
            """;
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call SearchByNameAsync with search term
        var result = await repository.SearchByNameAsync(searchTerm);
        var orderedResult = result.OrderByDescending(u =>
            u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        );

        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(orderedResult);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}
