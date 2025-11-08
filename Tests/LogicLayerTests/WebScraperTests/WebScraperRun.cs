using AngleSharp.Dom;
using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using savorfolio_backend.Models.DTOs;

namespace Tests.LogicLayerTests.WebScraperTests;

[Collection("Web Scraper collection")]
public partial class WebScraperRun() 
{
    private WebScraperService scraper = default!;
    // mock FallbackHeuristics interface
    private readonly Mock<IFallbackHeuristics> mockFallbackHeuristics = new();
    // mock fallback heuristic extensions interface
    private readonly Mock<IHeuristicExtensions> mockHeuristicExtensions = new();
    // mock IngredientParseService interface
    private readonly Mock<IIngredientParseService> mockIngredientParseService = new();
    // mock WebScraperService interface
    private readonly Mock<IWebScraperService> mockWebScraperService = new();

    [Fact]
    public void RunScraperAsyncCallsFunctions()
    {
        // initialize web scraper
        scraper = new WebScraperService(
            mockFallbackHeuristics.Object,
            mockIngredientParseService.Object,
            mockHeuristicExtensions.Object
        );

        // set up mock returns for dependent functions
        mockWebScraperService.Setup(r => r.GetHtmlAsync(It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<IDocument>());

        mockWebScraperService.Setup(r => r.SampleCssClasses(It.IsAny<IDocument>()))
            .Returns(It.IsAny<string>());

        mockWebScraperService.Setup(r => r.MapCssClassPatterns(It.IsAny<string>()))
            .Returns(It.IsAny<Dictionary<string, string?>?>());

        mockWebScraperService.Setup(r => r.BuildRecipeSummary(
            It.IsAny<IDocument>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ))
            .Returns(It.IsAny<RecipeDTO>());
        mockWebScraperService.Setup(r => r.BuildRecipeTags(
            It.IsAny<IDocument>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ))
            .Returns(It.IsAny<TagStringsDTO>());
        mockWebScraperService.Setup(r => r.BuildRecipeIngredients(
            It.IsAny<IDocument>(),
            It.IsAny<string>()
        ))
            .Returns(It.IsAny<List<string>>());
        mockWebScraperService.Setup(r => r.BuildRecipeInstructions(
            It.IsAny<IDocument>(),
            It.IsAny<string>()
        ))
            .Returns(It.IsAny<List<InstructionDTO>>());

        // call RunScraperAsync
        _ = scraper.RunScraperAsync(It.IsAny<string>());

        // assert GetHtmlAsync is called once
        mockWebScraperService.Verify(f => f.GetHtmlAsync(It.IsAny<string>()), Times.AtMostOnce);
        // assert SampleCssClasses is called once
        mockWebScraperService.Verify(f => f.SampleCssClasses(It.IsAny<IDocument>()), Times.AtMostOnce);
        // assert MapCssClassPatterns is called once
        mockWebScraperService.Verify(f => f.MapCssClassPatterns(It.IsAny<string>()), Times.AtMostOnce);
        // assert BuildRecipeSummary is called once
        mockWebScraperService.Verify(f => f.BuildRecipeSummary(
            It.IsAny<IDocument>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ), Times.AtMostOnce);
        // assert BuildRecipeTags is called once
        mockWebScraperService.Verify(f => f.BuildRecipeTags(
            It.IsAny<IDocument>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ), Times.AtMostOnce);
        // assert BuildRecipeIngredients is called once
        mockWebScraperService.Verify(f => f.BuildRecipeIngredients(
            It.IsAny<IDocument>(),
            It.IsAny<string>()
        ), Times.AtMostOnce);
        // assert BuildRecipeSummary is called once
        mockWebScraperService.Verify(f => f.BuildRecipeInstructions(
            It.IsAny<IDocument>(),
            It.IsAny<string>()
        ), Times.AtMostOnce);
    }
}