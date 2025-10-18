using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.DataAccess;
using Tests.Fixtures;

namespace Tests.DataAccessTests;

[Collection("Unit test database Collection")]
public class IngredientRepositoryTests(UnitDbFixture unitDbFixture) : IClassFixture<UnitDbFixture>
{
    private readonly UnitDbFixture _unitDbFixture = unitDbFixture;


    [Fact]
    public async Task IngredientSearchTest()
    {
        // initialize search term
        string searchTerm = "chicken";

        IngredientRepository repository = new(_unitDbFixture.Context);

        // initialize expected result as string, convert to JSON
        // string expectedJson = """
        // [
        //     {"Id": 143,"Name": "chicken","TypeId": 7,"IngredientCategory": "Protein"},
        //     {"Id": 144,"Name": "chicken breast","TypeId": 7,"IngredientCategory": "Protein"},
        //     {"Id": 251,"Name": "chicken broth","TypeId": 4,"IngredientCategory": "Broth & Stock"},
        //     {"Id": 20,"Name": "chicken stock","TypeId": 4,"IngredientCategory": "Broth & Stock"},
        //     {"Id": 145,"Name": "chicken thigh","TypeId": 7,"IngredientCategory": "Protein"}
        // ]
        // """;
        string expectedJson = """
        [
            {"Id":20,"Name":"chicken stock","TypeId":4,"IngredientCategory":"Broth & Stock"},
            {"Id":143,"Name":"chicken","TypeId":7,"IngredientCategory":"Protein"},
            {"Id":144,"Name":"chicken breast","TypeId":7,"IngredientCategory":"Protein"},
            {"Id":145,"Name":"chicken thigh","TypeId":7,"IngredientCategory":"Protein"},
            {"Id":251,"Name":"chicken broth","TypeId":4,"IngredientCategory":"Broth & Stock"}
        ]
        """;
        JToken expectedToken = JToken.Parse(expectedJson);

        // Call SearchByNameAsync with search term
        var result = await repository.SearchByNameAsync(searchTerm);
        var orderedResult = result.OrderByDescending(u =>
                u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(orderedResult);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));

    }
}