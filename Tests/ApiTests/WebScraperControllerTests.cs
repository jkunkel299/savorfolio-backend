using Moq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;

namespace Tests.ApiTests;

public class WebScraperControllerTests()
{
    [Fact]
    public async Task ScraperAPI_CallsDependent()
    {
        // mock web scraper service interface
        var mockWebScraperService = new Mock<IWebScraperService>();
        // set up return
        mockWebScraperService
            .Setup(r => r.RunScraperAsync(It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<DraftRecipeDTO>());
        // call mocked API endpoint
        var result = await RecipeEndpointsHelper.InvokeRecipeScrapeEndpoint(
            It.IsAny<string>(),
            mockWebScraperService.Object
        );
        // assert RunScraperAsync called once
        mockWebScraperService.Verify(d => d.RunScraperAsync(It.IsAny<string>()), Times.AtMostOnce);
        // assert result.Ok type
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok>(result);
    }
}
