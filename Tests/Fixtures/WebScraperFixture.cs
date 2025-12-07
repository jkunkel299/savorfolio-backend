using System.Collections.Concurrent;
using AngleSharp;
using AngleSharp.Dom;
using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using Tests.Helpers;

namespace Tests.Fixtures;

public class WebScraperFixture : IDisposable
{
    // public Mock<IWebScraperService> MockWebScraperService { get; private set; }
    // public IWebScraperService WebScraperService => MockWebScraperService.Object;

    private readonly ConcurrentDictionary<string, IDocument> _documentCache = new();

    // public WebScraperFixture()
    // {
    //     // mock units repository interface
    //     var mockUnitRepo = new Mock<IUnitsRepository>();
    //     // mock units repository interface
    //     var mockIngredientRepo = new Mock<IIngredientRepository>();
    //     // Initialize the mock once for all tests
    //     var mockWebScraperService = new WebScraperService(
    //         mockUnitRepo.Object,
    //         mockIngredientRepo.Object
    //     );
    // }

    public virtual async Task<IDocument> WebScraperSetupAsync(string filename)
    {
        // Normalize the key (case-insensitive)
        string cacheKey = filename.ToLowerInvariant();

        // If already cached, return it immediately
        if (_documentCache.TryGetValue(cacheKey, out var cachedDoc))
            return cachedDoc;

        string htmlFilePath = TestFileHelper.GetProjectPath($"HtmlFiles/{filename}");
        string htmlContent = File.ReadAllText(htmlFilePath);

        var newDoc = await FixtureHtmlAsync(htmlContent);

        // Store it in the cache
        _documentCache.TryAdd(cacheKey, newDoc);

        return newDoc;
    }

    public static async Task<IDocument> FixtureHtmlAsync(string htmlContent)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(htmlContent));
        // return IDocument
        return document;
    }

    public void Dispose()
    {
        _documentCache.Clear();
        GC.SuppressFinalize(this);
    }
}
