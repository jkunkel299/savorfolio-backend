using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using Tests.Fixtures;

namespace Tests.DataAccessTests;

[Collection("Unit test database Collection")]
public class UnitRepositoryTests(UnitDbFixture unitDbFixture) : IClassFixture<UnitDbFixture>
{
    // private readonly UnitDbFixture _unitDbFixture = unitDbFixture;
    private readonly UnitsRepository _repository = new(unitDbFixture.Context);
    

    [Fact]
    public async Task UnitSearchTest()
    {
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

        // Call SearchByNameAsync with search term
        var result = await _repository.SearchUnitTableAsync(searchTerm);

        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(result);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));

    }
}