using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using Tests.Fixtures;

namespace Tests.LogicLayerTests.WebScraperTests;

[Collection("Web Scraper collection")]
public partial class MvCreateWebScraperTests(WebScraperFixture webScraperFixture)
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
        _document = await webScraperFixture.WebScraperSetupAsync("mv-createPattern.html");
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
        string expectedPattern = "mv-create";

        // call SampleCssClasses on the document
        string actualPattern = scraper.SampleCssClasses(_document);

        // assert equal
        Assert.Equal(expectedPattern, actualPattern);
    }

    [Fact]
    public void MapCssClassPatterns_mvCreate()
    {
        // initialize string pattern
        string pattern = "mv-create";

        // initialize expected return
        var expectedPatterns = new Dictionary<string, string?>
        {
            { "RecipeTitle", "mv-create-title " },
            { "Description", "mv-create-description" },
            { "PrepTime", "mv-create-time-prep" },
            { "CookTime", "mv-create-time-active" },
            { "Servings", "mv-create-yield" },
            { "Ingredients", "mv-create-ingredients" },
            { "Instructions", "mv-create-instructions" },
            { "Course", "mv-create-category" },
            { "Cuisine", "mv-create-cuisine" },
        };

        // call MapCssClassPatterns
        var actualPatterns = scraper.MapCssClassPatterns(pattern);

        // assert equal
        Assert.Equal(expectedPatterns, actualPatterns);
    }

    [Fact]
    public void BuildRecipeSummary_mvCreate()
    {
        // initialize inputs
        string titlePattern = "mv-create-title ";
        string descriptionPattern = "mv-create-description";
        string prepTimePattern = "mv-create-time-prep";
        string cookTimePattern = "mv-create-time-active";
        string servingsPattern = "mv-create-yield";

        // initialize expected returns
        string recipeTitle = "BEST Jiffy Cornbread With Creamed Corn";
        string recipeDescription =
            "7-ingredient Creamed Corn Cornbread is a staple Southern side dish. Jiffy Cornbread with Creamed Corn is full of flavor, so easy to make, and will become an instant family favorite.";
        string recipePrep = "5 minutes";
        string recipeCook = "40 minutes";
        string recipeServings = "16";
        int? bakeTemp = 400;
        string tempUnit = "F";

        // bake temp and temp unit rely on fallback heuristics, and for isolation
        // purposes the ExtractBakeTemp returns must be mocked
        var tupleExtractBakeTemp = (bakeTemp, tempUnit);
        mockFallbackHeuristics
            .Setup(r => r.ExtractBakeTemp(_document))
            .Returns(tupleExtractBakeTemp);

        // call BuildRecipeSummary
        var actualReturn = scraper.BuildRecipeSummary(
            _document,
            titlePattern,
            descriptionPattern,
            prepTimePattern,
            cookTimePattern,
            servingsPattern
        );
        string cleanedServings = WhitespaceRegex.Replace(actualReturn.Servings!, " ");

        // assert elements are as expected
        Assert.Equal(recipeTitle, actualReturn.Name);
        Assert.Equal(recipeDescription, actualReturn.Description);
        Assert.Equal(recipePrep, actualReturn.PrepTime);
        Assert.Equal(recipeCook, actualReturn.CookTime);
        Assert.Equal(recipeServings, cleanedServings);
        Assert.Equal(bakeTemp, actualReturn.BakeTemp);
        Assert.Equal(tempUnit, actualReturn.Temp_unit);
    }

    // note that tests related to the ingredient parse service will be implemented separately
    // the test that the dependent function is called will be implemented in the no-pattern test suite

    [Fact]
    public void BuildRecipeInstructions_mvCreate()
    {
        // initialize input
        string instructionsPattern = "mv-create-instructions";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
            [
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 1, 
                    "InstructionText": "Preheat the oven to 400 F (204 C), and lightly grease an 8 x 8 baking dish with cooking spray."
                },
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 2,
                    "InstructionText": "In a large mixing bowl, whisk the eggs and oil together, then whisk in the milk and honey (make sure to give the honey a good mixing to combine well). Add in the cream style corn and sour cream, and mix to combine. Stir the Jiffy Corn Muffin Mix into the liquid until just combined (don't over mix it). "
                },
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 3,
                    "InstructionText": "Pour the batter into the greased baking dish and bake for 40 - 45 minutes. Check the cornbread after 40 minutes. To test if it’s ready, insert a toothpick into the center. If the toothpick comes out clean, it's done. If it comes out a bit wet, put in for a few more minutes. "
                }
            ]
            """;
        JToken expectedToken = JToken.Parse(expectedJson);

        // call BuildRecipeSummary
        var actualReturn = scraper.BuildRecipeInstructions(_document, instructionsPattern);
        foreach (var item in actualReturn)
        {
            var text = item.InstructionText;
            var cleaned = WhitespaceRegex.Replace(text, string.Empty);
            item.InstructionText = cleaned;
        }
        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(actualReturn);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [Fact]
    public void BuildRecipeTags_mvCreate()
    {
        // initialize inputs
        string coursePattern = "mv-create-category";
        string cuisinePattern = "mv-create-cuisine";

        // initialize expected returns for recipe_type and cuisine
        string expectedRecipeType = "Side";
        string expectedCuisine = "American";

        // Meal and Dietary rely on fallback heuristics, and for isolation
        // purposes will not be tested in this context

        // call BuildRecipeTags
        var actualReturn = scraper.BuildRecipeTags(_document, coursePattern, cuisinePattern);

        // Assert equal
        Assert.Equal(expectedRecipeType, actualReturn.Recipe_type);
        Assert.Equal(expectedCuisine, actualReturn.Cuisine);
    }

    private static readonly Regex WhitespaceRegex = new(
        @"\s{2,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
}
