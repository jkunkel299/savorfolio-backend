using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.LogicLayer.WebScraper;
using savorfolio_backend.Models.enums;
using Tests.Fixtures;

namespace Tests.LogicLayerTests.WebScraperTests;

[Collection("Web Scraper collection")]
public partial class WprmWebScraperTests(WebScraperFixture webScraperFixture) : IClassFixture<WebScraperFixture>, IAsyncLifetime
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
        _document = await webScraperFixture.WebScraperSetupAsync("wprmPattern.html");
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
    public void SampleCssClasses_wprm()
    {
        // initilize the expected return
        string expectedPattern = "wprm";

        // call SampleCssClasses on the document
        string actualPattern = scraper.SampleCssClasses(_document);

        // assert equal
        Assert.Equal(expectedPattern, actualPattern);
    }

    [Fact]
    public void MapCssClassPatterns_wprm()
    {
        // initialize string pattern
        string pattern = "wprm";

        // initialize expected return
        var expectedPatterns = new Dictionary<string, string?>
        {
            { "RecipeTitle", "wprm-recipe-name" },
            { "Description", "wprm-recipe-summary" },
            { "PrepTime", "wprm-recipe-prep_time" },
            { "CookTime", "wprm-recipe-cook_time" },
            { "Servings", "wprm-recipe-servings" },
            { "Ingredients", "wprm-recipe-ingredient" },
            { "Instructions", "wprm-recipe-instruction-group" },
            { "Course", "wprm-recipe-course " },
            { "Cuisine", "wprm-recipe-cuisine " },
        };

        // call MapCssClassPatterns
        var actualPatterns = scraper.MapCssClassPatterns(pattern);

        // assert equal
        Assert.Equal(expectedPatterns, actualPatterns);
    }

    [Fact]
    public void BuildRecipeSummary_wprm()
    {
        // initialize inputs
        string titlePattern = "wprm-recipe-name";
        string descriptionPattern = "wprm-recipe-summary";
        string prepTimePattern = "wprm-recipe-prep_time";
        string cookTimePattern = "wprm-recipe-cook_time";
        string servingsPattern = "wprm-recipe-servings";

        // initialize expected returns
        string recipeTitle = "Fall Chocolate Chip Spiced Cookie (Levain Bakery Fall Cookie)";
        string recipeDescription = "Soft, chewy Fall cookie with dark brown sugar, molasses, cinnamon, ginger, cloves, nutmeg, and chocolate chips for the perfect Autumn cookie. This is the perfect Fall spiced chocolate chip cookie recipe!";
        string recipePrep = "15 minutes";
        string recipeCook = "10 minutes";
        string recipeServings = "8";
        int bakeTemp = 400;
        string tempUnit = "F";

        // bake temp and temp unit rely on fallback heuristics, and for isolation 
        // purposes the ExtractBakeTemp returns must be mocked
        var tupleExtractBakeTemp = (bakeTemp, tempUnit);
        mockFallbackHeuristics.Setup(r => r.ExtractBakeTemp(_document))
            .Returns(tupleExtractBakeTemp);

        // call BuildRecipeSummary
        var actualReturn = scraper.BuildRecipeSummary(_document, titlePattern, descriptionPattern, prepTimePattern, cookTimePattern, servingsPattern);

        // assert elements are as expected
        Assert.Equal(recipeTitle, actualReturn.Name);
        Assert.Equal(recipeDescription, actualReturn.Description);
        Assert.Equal(recipePrep, actualReturn.PrepTime);
        Assert.Equal(recipeCook, actualReturn.CookTime);
        Assert.Equal(recipeServings, actualReturn.Servings);
        Assert.Equal(bakeTemp, actualReturn.BakeTemp);
        Assert.Equal(tempUnit, actualReturn.Temp_unit);
    }

    // note that tests related to the ingredient parse service will be implemented separately
    // the test that the dependent function is called will be implemented in the no-pattern test suite

    [Fact]
    public void BuildRecipeInstructions_wprm()
    {
        // initialize input
        string instructionsPattern = "wprm-recipe-instruction-group";

        // initialize expected result as string, convert to JSON
        string expectedJson = """
        [
            {
                "Id": 0,
                "RecipeId": 0,
                "SectionId": null,
                "SectionName": null,
                "StepNumber": 1, 
                "InstructionText": "Preheat oven to 400 degrees. In a large mixing bowl, beat butter, brown sugar, and sugar for 4 minutes until light and fluffy."
            },
            {
                "Id": 0,
                "RecipeId": 0,
                "SectionId": null,
                "SectionName": null,
                "StepNumber": 2,
                "InstructionText": "Add molasses and egg and mix for 1 minute longer."
            },
            {
                "Id": 0,
                "RecipeId": 0,
                "SectionId": null,
                "SectionName": null,
                "StepNumber": 3,
                "InstructionText": "Fold in flour, baking soda, cornstarch, salt, cinnamon, ginger, nutmeg, cloves, and chocolate chips."
            },
            {
                "Id": 0,
                "RecipeId": 0,
                "SectionId": null,
                "SectionName": null,
                "StepNumber": 4,
                "InstructionText": "Roll into 4-ounce, 5-ounce, or 6-ounce balls. Place on a parchment paper lined baking sheet. I prefer to use light-colored baking sheets."
            },
            {
                "Id": 0,
                "RecipeId": 0,
                "SectionId": null,
                "SectionName": null,
                "StepNumber": 5,
                "InstructionText": "Bake for 8-10 minutes. The cookies will still be slightly underdone when you remove them from the oven. Let the cookies sit for 10-15 minutes before moving them from the baking sheet."
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
    public void BuildRecipeTags_wprm()
    {
        // initialize inputs
        string coursePattern = "wprm-recipe-course ";
        string cuisinePattern = "wprm-recipe-cuisine ";

        // initialize expected returns for recipe_type and cuisine
        string expectedRecipeType = "Dessert";
        string expectedCuisine = "American";

        // Meal and Dietary rely on fallback heuristics, and for isolation 
        // purposes this context will only test that they are called once each
        // However, due to change in the web scraper, dietary tags will be excluded.
        // set up returns for fallback functions called in BuildRecipeInstructions
        mockHeuristicExtensions.Setup(r => r.MatchEnum<MealTag>(_document))
            .Returns(It.IsAny<string>());
        // mockFallbackHeuristics.Setup(r => r.ExtractDietaryTags(It.IsAny<string>()))
        //     .Returns(It.IsAny<List<string>>());

        // call BuildRecipeTags
        var actualReturn = scraper.BuildRecipeTags(_document, coursePattern, cuisinePattern);

        // Assert equal
        Assert.Equal(expectedRecipeType, actualReturn.Recipe_type);
        Assert.Equal(expectedCuisine, actualReturn.Cuisine);

        // assert fallbacks are called once
        mockHeuristicExtensions.Verify(f => f.MatchEnum<MealTag>(_document), Times.AtMostOnce);
        // mockFallbackHeuristics.Verify(f => f.ExtractDietaryTags(It.IsAny<string>()), Times.AtMostOnce);
    }

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex WhitespaceRegex();
}