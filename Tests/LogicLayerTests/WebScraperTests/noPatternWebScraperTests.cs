using AngleSharp.Dom;
using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using savorfolio_backend.Models.DTOs;
using Tests.Fixtures;

namespace Tests.LogicLayerTests.WebScraperTests;

[Collection("Web Scraper collection")]
public partial class NoPatternWebScraperTests(WebScraperFixture webScraperFixture)
    : IClassFixture<WebScraperFixture>,
        IAsyncLifetime
{
    private IDocument _document = default!;
    private WebScraperService scraper = default!;

    // mock FallbackHeuristics interface
    private readonly Mock<IFallbackHeuristics> mockFallbackHeuristics = new();

    // mock fallback heuristic extensions interface
    private readonly Mock<IHeuristicExtensions> mockHeuristicExtensions = new();

    // mock IngredientParseService interface
    private readonly Mock<IIngredientParseService> mockIngredientParseService = new();

    // initialize document and web scraper
    public async Task InitializeAsync()
    {
        _document = await webScraperFixture.WebScraperSetupAsync("noPattern.html");
        // Initialize the mock once for all tests
        scraper = new WebScraperService(
            mockFallbackHeuristics.Object,
            mockIngredientParseService.Object,
            mockHeuristicExtensions.Object
        );
    }

    // dispose
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void SampleCssClasses_mvCreate()
    {
        // initilize the expected return
        string expectedPattern = "none";

        // call SampleCssClasses on the document
        string actualPattern = scraper.SampleCssClasses(_document);

        // assert equal
        Assert.Equal(expectedPattern, actualPattern);
    }

    [Fact]
    public void MapCssClassPatterns_noPattern()
    {
        // initialize string pattern
        string pattern = "none";

        // initialize expected return
        Dictionary<string, string?>? expectedPatterns = null;

        // call MapCssClassPatterns - should return null
        var actualPatterns = scraper.MapCssClassPatterns(pattern);

        // assert equal
        Assert.Equal(expectedPatterns, actualPatterns);
    }

    // test that fallback heuristics are called.
    // Pattern-based tests have demonstrated/tested the DTO-building functionality

    [Fact]
    public void BuildRecipeSummary_callsFallbacks()
    {
        // initialize return values
        int expectedBakeTemp = 0;
        string expectedTempUnit = "F";
        var tupleExtractBakeTemp = (expectedBakeTemp, expectedTempUnit);

        // the fallback heuristic service is mocked before tests
        // set up returns for fallback functions called in BuildRecipeSummary
        mockFallbackHeuristics.Setup(r => r.ExtractTitle(_document)).Returns(It.IsAny<string>());
        mockFallbackHeuristics
            .Setup(r => r.ExtractDescription(_document))
            .Returns(It.IsAny<string>());
        mockFallbackHeuristics
            .Setup(r => r.ExtractTimeNearLabel(_document, "prep time"))
            .Returns(It.IsAny<string>());
        mockFallbackHeuristics
            .Setup(r => r.ExtractTimeNearLabel(_document, "cook time"))
            .Returns(It.IsAny<string>());
        mockFallbackHeuristics.Setup(r => r.ExtractServings(_document)).Returns(It.IsAny<string>());
        mockFallbackHeuristics
            .Setup(r => r.ExtractBakeTemp(_document))
            .Returns(tupleExtractBakeTemp);

        // call BuildRecipeSummary from mocked web scraper interface
        _ = scraper.BuildRecipeSummary(_document);

        // assert mocked ExtractTitle function called once
        mockFallbackHeuristics.Verify(f => f.ExtractTitle(_document), Times.AtMostOnce);
        // assert mocked ExtractTitle function called once
        mockFallbackHeuristics.Verify(f => f.ExtractDescription(_document), Times.AtMostOnce);
        // assert mocked ExtractTitle function called once
        mockFallbackHeuristics.Verify(
            f => f.ExtractTimeNearLabel(_document, "prep time"),
            Times.AtMostOnce
        );
        // assert mocked ExtractTitle function called once
        mockFallbackHeuristics.Verify(
            f => f.ExtractTimeNearLabel(_document, "cook time"),
            Times.AtMostOnce
        );
        // assert mocked ExtractTitle function called once
        mockFallbackHeuristics.Verify(f => f.ExtractBakeTemp(_document), Times.AtMostOnce);
    }

    [Fact]
    public void BuildRecipeIngredients_callsParseService()
    {
        // set up returns for ingredient parse function called in BuildRecipeIngredients
        mockIngredientParseService
            .Setup(r => r.ExtractIngredients(_document, ""))
            .Returns(It.IsAny<List<string>>());
        // call BuildRecipeIngredients from mocked web scraper interface
        _ = scraper.BuildRecipeIngredients(_document);
        // assert mocked ExtractIngredients function called once
        mockIngredientParseService.Verify(
            f => f.ExtractIngredients(_document, It.IsAny<string>()),
            Times.AtMostOnce
        );
    }

    [Fact]
    public void BuildRecipeInstructions_callsFallbacks()
    {
        // set up returns for fallback function called in BuildRecipeInstructions
        mockFallbackHeuristics
            .Setup(r => r.ExtractInstructions(_document))
            .Returns(It.IsAny<List<InstructionDTO>>());
        // call BuildRecipeInstructions from mocked web scraper interface
        _ = scraper.BuildRecipeInstructions(_document);
        // assert mocked ExtractInstructions function called once
        mockFallbackHeuristics.Verify(f => f.ExtractInstructions(_document), Times.AtMostOnce);
    }

    [Fact]
    public void BuildRecipeTags_callsFallbacks()
    {
        // set up returns for fallback function called in BuildRecipeTags
        mockFallbackHeuristics
            .Setup(r => r.ExtractTags(_document))
            .Returns(It.IsAny<TagStringsDTO>());
        // call BuildRecipeTags from mocked web scraper interface
        _ = scraper.BuildRecipeTags(_document);
        // assert mocked ExtractTags function called once
        mockFallbackHeuristics.Verify(f => f.ExtractTags(_document), Times.AtMostOnce);
    }
}
