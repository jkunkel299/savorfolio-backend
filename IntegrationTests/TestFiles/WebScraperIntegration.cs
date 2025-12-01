using System.Net;
using System.Text;
using IntegrationTests.Fixtures;
using IntegrationTests.TestData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tests.Helpers;

namespace IntegrationTests.TestFiles;

[Collection("Integration Test Server")]
public class WebScraperIntegrationTests(TestServerFixture testServerFixture)
    : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client = testServerFixture.HttpClient;

    [Theory]
    [MemberData(
        nameof(WebScraperIntegrationData.WebScraperTestCases),
        MemberType = typeof(WebScraperIntegrationData)
    )]
    public async Task WebScraper_ReturnsExpected(string url, string filePath)
    {
        // initialize document from filepath as JSON
        string scrapeRecipeFilePath = TestFileHelper.GetProjectPath(filePath);
        JObject expectedRecipe = JObject.Parse(File.ReadAllText(scrapeRecipeFilePath));
        var expectedJson = JsonConvert.SerializeObject(expectedRecipe);
        JToken expectedToken = JToken.Parse(expectedJson);

        // Prepare HTTP content with url
        var content = new StringContent($"\"{url}\"", Encoding.UTF8, "application/json");

        // call the API endpoint
        var response = await _client.PostAsync("/api/recipes/add/scrape", content);
        // get the response content/message
        var actualMessage = await response.Content.ReadAsStringAsync();
        // convert to JSON token
        JToken actualToken = JToken.Parse(actualMessage);

        // Assert status code 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert results not null
        Assert.NotNull(actualMessage);

        // Assert equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }
}
