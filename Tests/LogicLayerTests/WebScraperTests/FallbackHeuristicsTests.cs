using AngleSharp;
using AngleSharp.Dom;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Models.enums;
using Tests.TestData;

namespace Tests.LogicLayerTests.WebScraperTests;

// [Collection("Web Scraper collection")]
public class FallbackHeuristicsTests
{
    private readonly Mock<IFallbackHeuristics> mockFallbackHeuristics = new();
    private readonly Mock<IHeuristicExtensions> mockHeuristicExtensions;
    private readonly FallbackHeuristics _fallback;

    public FallbackHeuristicsTests()
    {
        mockHeuristicExtensions = new Mock<IHeuristicExtensions>();
        _fallback = new(mockHeuristicExtensions.Object);
    }
    
    #region Title
    [Theory]
    [InlineData( // Includes a title matching the Title Regex
        @"<html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
        </body></html>",
        "Cinnamon Chocolate Babka Muffins"
    )]
    [InlineData( // Only includes a title in a meta tag
        @"<html><head>
            <meta property='og:title' content='Cinnamon Chocolate Babka Muffins' />
        </head>
        <body>
            <p>No match in the body</p>
        </body></html>",
        "Cinnamon Chocolate Babka Muffins"
    )]
    [InlineData( // No match
        @"<html><body>
            <h1>No matching class here</h1>
        </body></html>",
        ""
    )]
    public async Task ExtractTitle_ReturnsExpectedAsync(string html, string expectedTitle)
    {
        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // call ExtractTitle on the generated document
        string actualResult = _fallback.ExtractTitle(document);

        // assert that the actual result is equal to the expected result
        Assert.Equal(expectedTitle, actualResult);
    }
    #endregion

    #region Description
    [Theory]
    [InlineData( // class name including “recipe-description”
        @"<!DOCTYPE html>
        <html><body>
            <div class='recipe-description'>A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!</div>
        </body></html>",
        "A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!"
    )]
    [InlineData( // class name including “summary"
        @"<!DOCTYPE html>
        <html><body>
            <div class='recipe-summary-content'>A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!</div>
        </body></html>",
        "A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!"
    )]
    [InlineData( // near an element matching the Title Regex
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Best Pancakes</h1>
            <p>A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!</p>
        </body></html>",
        "A rich, buttery babka yeast dough, filled with beautiful swirls of chocolate, baked in muffin tins. Same great babka taste, baked in half the time!"
    )]
    [InlineData( // no match
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <h2>Ingredients</h2>
        </body></html>",
        ""
    )]
    public async Task ExtractDescription_ReturnsExpectedAsync(string html, string expectedDescription)
    {
        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // call ExtractDescription on the generated document
        string actualResult = _fallback.ExtractDescription(document);

        // assert that the actual result is equal to the expected result
        Assert.Equal(expectedDescription, actualResult);
    }
    #endregion

    #region Time
    [Fact]
    public async Task ExtractTime_CallsFunctionsAsync()
    {
        // initialize HTML snippet
        string html = @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Prep Time: 3 hours 30 minutes</div>
        </body></html>";

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for GetBestMatch (dependent function)
        mockHeuristicExtensions.Setup(r => r.GetBestMatch(It.IsAny<IEnumerable<IElement>>(), It.IsAny<string>()))
            .Returns("3 hours 30 minutes");

        // call ExtractTimeNearLabel on a mocked document
        _ = _fallback.ExtractTimeNearLabel(document, "prep time");

        // assert the mocked function was called once
        mockHeuristicExtensions.Verify(f => f.GetBestMatch(It.IsAny<IEnumerable<IElement>>(), It.IsAny<string>()), Times.AtMostOnce);
    }

    [Theory]
    [InlineData( // prep time matches prep time
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Prep Time: 3 hours 30 minutes</div>
        </body></html>",
        "prep time",
        "Prep Time: 3 hours 30 minutes",
        "3 hours 30 minutes"
    )]
    [InlineData( // cook time matches cook time
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Cook Time: 25 minutes</div>
        </body></html>",
        "cook time",
        "Cook Time: 25 minutes",
        "25 minutes"
    )]
    [InlineData( // prep time no match
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>No time</div>
        </body></html>",
        "prep time",
        "",
        ""
    )]
    [InlineData( // cook time no match
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>No time</div>
        </body></html>",
        "cook time",
        "",
        ""
    )]
    public async Task ExtractTimeNearLabel_ReturnsExpectedAsync(string html, string label, string bestMatch, string expected)
    {
        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for GetBestMatch (dependent function)
        mockHeuristicExtensions.Setup(r => r.GetBestMatch(It.IsAny<IEnumerable<IElement>>(), It.IsAny<string>()))
            .Returns(bestMatch);

        // call ExtractTimeNearLabel on the generated document
        string actualResult = _fallback.ExtractTimeNearLabel(document, label);

        // assert that the actual result is equal to the expected result
        Assert.Equal(expected, actualResult);
    }
    #endregion

    #region Servings
    [Fact]
    public async Task ExtractServings_CallsFunctionsAsync()
    {
        // initialize HTML snippet
        string html = @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Prep Time: 3 hours 30 minutes</div>
        </body></html>";

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for GetBestMatch (dependent function)
        mockHeuristicExtensions.Setup(r => r.GetBestMatch(It.IsAny<IEnumerable<IElement>>(), It.IsAny<string>()))
            .Returns(It.IsAny<string>());

        // call ExtractServings on a mocked document
        _ = _fallback.ExtractServings(document);

        // assert the mocked function was called once
        mockHeuristicExtensions.Verify(f => f.GetBestMatch(It.IsAny<IEnumerable<IElement>>(), It.IsAny<string>()), Times.AtMostOnce);
    }
    
    [Theory]
    [InlineData( // "servings"
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Servings: 12 muffins</div>
        </body></html>",
        "Servings: 12 muffins",
        "12"
    )]
    [InlineData( // "serves"
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Serves: 12 muffins</div>
        </body></html>",
        "Serves: 12 muffins",
        "12"
    )]
    [InlineData( // "yield"
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Yield: 12 muffins</div>
        </body></html>",
        "Yield: 12 muffins",
        "12"
    )]
    [InlineData( // "yields"
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Yields: 12 muffins</div>
        </body></html>",
        "Yields: 12 muffins",
        "12"
    )]
    [InlineData( // no match
        @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>No servings</div>
        </body></html>",
        "",
        ""
    )]
    public async Task ExtractServings_ReturnsExpectedAsync(string html, string bestMatch, string expected)
    {
        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for GetBestMatch (dependent function)
        mockHeuristicExtensions.Setup(r => r.GetBestMatch(It.IsAny<IEnumerable<IElement>>(), It.IsAny<string>()))
            .Returns(bestMatch);

        // call ExtractServings on the generated document
        string actualResult = _fallback.ExtractServings(document);

        // assert that the actual result is equal to the expected result
        Assert.Equal(expected, actualResult);
    }
    #endregion

    #region Tags
    [Fact]
    public async Task ExtractTags_CallsFunctionsAsync()
    {
        // initialize HTML snippet
        string html = @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Cuisine: American</div>
            <div>Meal: Breakfast</div>
            <div>Course: Side</div>
        </body></html>";

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for MatchEnum<RecipeTypeTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<RecipeTypeTag>(document))
            .Returns(It.IsAny<string>());
        // set up return for MatchEnum<CuisineTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<CuisineTag>(document))
            .Returns(It.IsAny<string>());
        // set up return for MatchEnum<MealTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<MealTag>(document))
            .Returns(It.IsAny<string>());

        // call ExtractTags on a mocked document
        _ = mockFallbackHeuristics.Object.ExtractTags(document);

        // assert each mocked function was called once
        mockHeuristicExtensions.Verify(f => f.MatchEnum<RecipeTypeTag>(document), Times.AtMostOnce);
        mockHeuristicExtensions.Verify(f => f.MatchEnum<CuisineTag>(document), Times.AtMostOnce);
        mockHeuristicExtensions.Verify(f => f.MatchEnum<MealTag>(document), Times.AtMostOnce);
    }

    [Fact]
    public async Task ExtractTags_ReturnsEmptyAsync()
    {
        // initialize empty HTML
        string html = @"<!DOCTYPE html>
        <html><head>
        </head></html>";
        // initialize expected return
        TagStringsDTO expectedReturn = new();
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedReturn);
        JToken expectedToken = JToken.Parse(expectedJson);

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for MatchEnum<RecipeTypeTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<RecipeTypeTag>(document))
            .Returns(It.IsAny<string>());
        // set up return for MatchEnum<CuisineTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<CuisineTag>(document))
            .Returns(It.IsAny<string>());
        // set up return for MatchEnum<MealTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<MealTag>(document))
            .Returns(It.IsAny<string>());

        // call ExtractTags on the empty document
        var actualReturn = _fallback.ExtractTags(document);
        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(actualReturn);
        JToken actualToken = JToken.Parse(actualJson);

        // assert actual return equal to expected
        Assert.Equal(expectedToken, actualToken);
    }

    [Fact]
    public async Task ExtractTags_CompilesDTO()
    {
        // initialize HTML snippet
        string html = @"<!DOCTYPE html>
        <html><body>
            <h1 class='recipe-title'>Cinnamon Chocolate Babka Muffins</h1>
            <div>Cuisine: American</div>
            <div>Meal: Breakfast</div>
            <div>Course: Side</div>
        </body></html>";
        // initialize expected return
        var expectedReturn = new TagStringsDTO
        {
            Recipe_type = "Side",
            Cuisine = "American",
            Meal = "Breakfast"
        };
        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedReturn);
        JToken expectedToken = JToken.Parse(expectedJson);

        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // set up return for MatchEnum<RecipeTypeTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<RecipeTypeTag>(It.IsAny<IDocument>()))
            .Returns("Side");
        // set up return for MatchEnum<CuisineTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<CuisineTag>(It.IsAny<IDocument>()))
            .Returns("American");
        // set up return for MatchEnum<MealTag> (dependent function)
        mockHeuristicExtensions.Setup(r => r.MatchEnum<MealTag>(It.IsAny<IDocument>()))
            .Returns("Breakfast");

        // call ExtractTags
        var actualReturn = _fallback.ExtractTags(document);
        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(actualReturn);
        JToken actualToken = JToken.Parse(actualJson);

        // assert actual return equal to expected
        Assert.Equal(expectedToken, actualToken);
    }
    #endregion

    #region Instructions
    [Theory]
    [MemberData(nameof(FallbackInstructionData.InstructionTestCases), MemberType = typeof(FallbackInstructionData))]
    public async Task ExtractInstructions_ReturnsExpected(string html, InstructionDTO[] expectedArray)
    {
        var expectedReturn = expectedArray.ToList();
        // use AngleSharp to build a DOM from the HTML snippet
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html));

        // convert to JSON
        var expectedJson = JsonConvert.SerializeObject(expectedReturn);
        JToken expectedToken = JToken.Parse(expectedJson);

        // call ExtractInstructions on the generated document
        var actualReturn = _fallback.ExtractInstructions(document);
        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(actualReturn);
        JToken actualToken = JToken.Parse(actualJson);

        // assert actual return equal to expected
        Assert.Equal(expectedToken, actualToken);
    }    
    #endregion
}