using System.Diagnostics.CodeAnalysis;
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
public partial class TastyWebScraperTests(WebScraperFixture webScraperFixture)
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
        _document = await webScraperFixture.WebScraperSetupAsync("tastyPattern.html");
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
    public void SampleCssClasses_tasty()
    {
        // initilize the expected return
        string expectedPattern = "tasty";

        // call SampleCssClasses on the document
        string actualPattern = scraper.SampleCssClasses(_document);

        // assert equal
        Assert.Equal(expectedPattern, actualPattern);
    }

    [Fact]
    public void MapCssClassPatterns_tasty()
    {
        // initialize string pattern
        string pattern = "tasty";

        // initialize expected return
        var expectedPatterns = new Dictionary<string, string?>
        {
            { "RecipeTitle", "tasty-recipes-title" },
            { "Description", "tasty-recipes-description-body" },
            { "PrepTime", "tasty-recipes-prep-time" },
            { "CookTime", "tasty-recipes-cook-time" },
            { "Servings", "tasty-recipes-yield" },
            { "Ingredients", "tasty-recipes-ingredients-body" },
            { "Instructions", "tasty-recipes-instructions-body" },
            { "Course", "tasty-recipes-category" },
            { "Cuisine", "tasty-recipes-cuisine" },
        };

        // call MapCssClassPatterns
        var actualPatterns = scraper.MapCssClassPatterns(pattern);

        // assert equal
        Assert.Equal(expectedPatterns, actualPatterns);
    }

    [Fact]
    public void BuildRecipeSummary_tasty()
    {
        // initialize inputs
        string titlePattern = "tasty-recipes-title";
        string descriptionPattern = "tasty-recipes-description-body";
        string prepTimePattern = "tasty-recipes-prep-time";
        string cookTimePattern = "tasty-recipes-cook-time";
        string servingsPattern = "tasty-recipes-yield";

        // initialize expected returns
        string recipeTitle = "Mushroom Stroganoff (Vegetarian)";
        string recipeDescription =
            "This vegetarian Mushroom Stroganoff recipe is quick and easy to make in about 30 minutes, and it is perfectly comforting, hearty, savory, and delicious. Feel free to serve over egg noodles, traditional pasta, quinoa, veggies, or whatever sounds delicious.";
        string recipePrep = "15 minutes";
        string recipeCook = "15 minutes";
        string recipeServings = "4 -6 1x";
        int? bakeTemp = null;
        string? tempUnit = null;

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
    public void BuildRecipeInstructions_tasty()
    {
        // initialize input
        string instructionsPattern = "tasty-recipes-instructions-body";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
            [
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 1, 
                    "InstructionText": "Cook egg noodles al dente in boiling, generously-salted water according to package instructions. (For optimal timing, I recommend actually adding the egg noodles to the boiling water at the same time that the vegetable stock is added to the stroganoff.)"
                },
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 2,
                    "InstructionText": "Melt 1 tablespoon butter in a large sauté pan over medium-high heat.Add onions and sauté for 5 minutes, stirring occasionally. Add the remaining 2 tablespoons butter, garlic and mushrooms, and stir to combine. Continue sautéing for an additional 5-7 minutes, until the mushrooms are cooked and tender. Add the white wine, and deglaze the pan by using a wooden spoon to scrape the brown bits off the bottom of the pan. Let the sauce simmer for 3 minutes."
                },
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 3,
                    "InstructionText": "Meanwhile, in a separate bowl, whisk together the vegetable stock, Worcestershire and flour until smooth. Pour the vegetable stock mixture into the pan, along with the thyme, and stir to combine. Let the mixture simmer for an additional 5 minutes, stirring occasionally, until slightly thickened. Then, stir in the Greek yogurt (or sour cream) evenly into the sauce. Taste, and season with a generous pinch of two of salt and pepper as needed."
                },
                {
                    "Id": 0,
                    "RecipeId": 0,
                    "SectionId": null,
                    "SectionName": null,
                    "StepNumber": 4,
                    "InstructionText": "Serve immediately over the egg noodles, garnished with your desired toppings."
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
    public void BuildRecipeTags_tasty()
    {
        // initialize inputs
        string coursePattern = "tasty-recipes-category";
        string cuisinePattern = "tasty-recipes-cuisine";

        // initialize expected returns for recipe_type and cuisine
        string expectedRecipeType = "Main";
        string expectedCuisine = "Italian";

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
