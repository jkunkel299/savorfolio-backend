using System.Text.RegularExpressions;
using AngleSharp;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using Tests.Fixtures;
using Tests.TestData;

namespace Tests.LogicLayerTests.WebScraperTests;

[Collection("Web Scraper collection")]
public partial class IngredientParseServiceTests(WebScraperFixture webScraperFixture) : IClassFixture<WebScraperFixture>
{
    private readonly Mock<IIngredientParseService> mockIngParseService = new();
    private readonly Mock<IFallbackHeuristics> mockFallbackHeuristics = new();
    private static readonly Mock<IHeuristicExtensions> mockHeuristicExtensions = new();
    private static readonly Mock<IUnitsRepository> mockUnitsRepository = new();
    private static readonly Mock<IIngredientRepository> mockIngredientRepository = new();

    private readonly IngredientParseService _service = new(
        mockUnitsRepository.Object,
        mockIngredientRepository.Object
    );

    #region ExtractIngredients
    [Theory]
    [InlineData("tasty-recipes-ingredients-body")]
    [InlineData("")]
    public async Task ExtractIngredients_CallsFunctionsAsync(string pattern)
    {
        // initialize HTML snippet
        string html = @"<!DOCTYPE html>
        <html><body>
            <h1>Cinnamon Chocolate Babka Muffins</h1>
            <div class='recipe-description'>A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!</div>
            <ul>
                <li>2 1/4 cups (270 grams) all purpose flour</li>
                <li>1/2 teaspoon salt</li>
            </ul>
        </body></html>";

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // mock dependent functions and returns
        mockIngParseService.Setup(r => r.GetIngredientsByPattern(document, It.IsAny<string>()))
            .Returns(It.IsAny<List<string>>());
        mockIngParseService.Setup(r => r.IngredientFallback(document))
            .Returns(It.IsAny<List<string>>());

        // call ExtractIngredients on the document
        _ = _service.ExtractIngredients(document, pattern);

        if (pattern != "")
        {
            // if the pattern is not empty, assert GetIngredientsByPattern was called once
            mockIngParseService.Verify(f => f.GetIngredientsByPattern(document, It.IsAny<string>()), Times.AtMostOnce);
        }
        if (pattern == "")
        {
            // if the pattern is empty, assert IngredientFallback was called once
            mockIngParseService.Verify(f => f.IngredientFallback(document), Times.AtMostOnce);
        }
    }
    #endregion

    #region GetIngredientsByPattern 
    [Theory]
    [MemberData(nameof(GetIngByPatternData.GetIngByPatternTestCases), MemberType = typeof(GetIngByPatternData))]
    public async Task GetIngredientsByPattern_ReturnsExpectedAsync(string htmlFilePath, string pattern, string[] expected)
    {
        // initialize document
        var _document = await webScraperFixture.WebScraperSetupAsync(htmlFilePath);

        // initialize expected ingredient return
        var expectedReturn = expected.ToList();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedReturn);
        JToken expectedToken = JToken.Parse(expectedJson);


        // call ExtractInstructions
        var actualReturn = _service.GetIngredientsByPattern(_document, pattern);
        // clean the strings, artifact from loading HTML markup locally
        for (int i = 0; i < actualReturn.Count; i++)
        {
            var cleaned = WhitespaceRegex().Replace(actualReturn[i], string.Empty);
            actualReturn[i] = cleaned;
        }
        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(actualReturn);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));

        // assert equal
        Assert.Equal(expectedReturn, actualReturn);
    }
    #endregion

    #region IngredientsFallback 
    [Theory]
    [MemberData(nameof(IngredientFallbackData.IngredientFallbackTestCases), MemberType = typeof(IngredientFallbackData))]
    public async Task IngredientsFallback_ReturnsExpectedAsync(string html, string[] expected)
    {
        // initialize expected ingredients return
        var expectedReturn = expected.ToList();

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // call IngredientsFallback
        var actualReturn = _service.IngredientFallback(document);

        // assert equal
        Assert.Equal(expectedReturn, actualReturn);
    }
    #endregion
    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex WhitespaceRegex();
}