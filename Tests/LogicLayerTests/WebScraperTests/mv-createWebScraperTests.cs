using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using savorfolio_backend.Models.DTOs;
using Tests.Fixtures;

namespace Tests.LogicLayerTests.WebScraperTests;

[Collection("Web Scraper collection")]
public partial class MvCreateWebScraperTests(WebScraperFixture webScraperFixture) : IClassFixture<WebScraperFixture>, IAsyncLifetime
{
    private IDocument _document = default!;
    private WebScraperService scraper = default!;

    // initialize document and web scraper
    public async Task InitializeAsync()
    {
        // var scraper = webScraperFixture.WebScraperService;
        _document = await webScraperFixture.WebScraperSetupAsync("mv-createPattern.html");
        // mock units repository interface
        var mockUnitRepo = new Mock<IUnitsRepository>();
        // mock units repository interface
        var mockIngredientRepo = new Mock<IIngredientRepository>();
        // Initialize the mock once for all tests
        scraper = new WebScraperService(
            mockUnitRepo.Object,
            mockIngredientRepo.Object
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
            { "PrepTime", "mv-create-time-prep .mv-time-minutes" },
            { "CookTime", "mv-create-time-active .mv-time-minutes" },
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
        string prepTimePattern = "mv-create-time-prep .mv-time-minutes";
        string cookTimePattern = "mv-create-time-active .mv-time-minutes";
        string servingsPattern = "mv-create-yield";

        // initialize expected returns
        string recipeTitle = "BEST Jiffy Cornbread With Creamed Corn";
        string recipeDescription = "7-ingredient Creamed Corn Cornbread is a staple Southern side dish. Jiffy Cornbread with Creamed Corn is full of flavor, so easy to make, and will become an instant family favorite.";
        string recipePrep = "5 minutes";
        string recipeCook = "40 minutes";
        string recipeServings = "16";
        int? bakeTemp = 400;
        string? tempUnit = "F";

        // call BuildRecipeSummary
        var actualReturn = scraper.BuildRecipeSummary(_document, titlePattern, descriptionPattern, prepTimePattern, cookTimePattern, servingsPattern);
        string cleanedServings = WhitespaceRegex().Replace(actualReturn.Servings!, " ");

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
            var cleaned = WhitespaceRegex().Replace(text, string.Empty);
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

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        {
            "RecipeId": 0,
            "Meal": "Breakfast",
            "Recipe_type": "Side",
            "Cuisine": "American",
            "Dietary": [
                "Vegan",
                "Vegetarian",
                "Dairy-Free",
                "Keto"
            ]
        }
        """;
        // the returned meal tag being "breakfast" instead of "lunch" or "dinner", 
        // as well as the erroneous dietary tags, are known issues
        // and each is an area of improvement for the meal tag and dietary tag extraction heuristics
        JToken expectedToken = JToken.Parse(expectedJson);

        // call BuildRecipeTags
        var actualReturn = scraper.BuildRecipeTags(_document, coursePattern, cuisinePattern);
        // Convert Result to JSON
        var actualJson = JsonConvert.SerializeObject(actualReturn);
        JToken actualToken = JToken.Parse(actualJson);

        // Assert Equal
        Assert.True(JToken.DeepEquals(expectedToken, actualToken));
    }

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex WhitespaceRegex();
}