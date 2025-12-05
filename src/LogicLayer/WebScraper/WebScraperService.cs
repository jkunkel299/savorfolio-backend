using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using FuzzySharp;
using FuzzySharp.Extractor;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using savorfolio_backend.Models.enums;
using savorfolio_backend.Utils;

namespace savorfolio_backend.LogicLayer.WebScraper;

public class WebScraperService(
    IFallbackHeuristics fallbackHeuristics,
    IIngredientParseService ingredientParseService,
    IHeuristicExtensions heuristicExtensions
) : IWebScraperService
{
    private readonly IFallbackHeuristics _fallbackHeuristics = fallbackHeuristics;
    private readonly IHeuristicExtensions _heuristicExtensions = heuristicExtensions;
    private readonly IIngredientParseService _ingredientParseService = ingredientParseService;

    #region Run Scraper
    public async Task<DraftRecipeDTO> RunScraperAsync(string url)
    {
        var document = await GetHtmlAsync(url);
        string patternMatch = SampleCssClasses(document);
        var patterns = MapCssClassPatterns(patternMatch);

        string titlePattern = patterns?["RecipeTitle"] ?? "";
        string descriptionPattern = patterns?["Description"] ?? "";
        string prepTimePattern = patterns?["PrepTime"] ?? "";
        string cookTimePattern = patterns?["CookTime"] ?? "";
        string servingsPattern = patterns?["Servings"] ?? "";
        string instructionsPattern = patterns?["Instructions"] ?? "";
        string coursePattern = patterns?["Course"]! ?? "";
        string cuisinePattern = patterns?["Cuisine"]! ?? "";
        string ingredientsPattern = patterns?["Ingredients"] ?? "";

        RecipeDTO recipe = BuildRecipeSummary(
            document,
            titlePattern,
            descriptionPattern,
            prepTimePattern,
            cookTimePattern,
            servingsPattern
        );
        TagStringsDTO tags = BuildRecipeTags(document, coursePattern, cuisinePattern);
        List<string> ingredients = BuildRecipeIngredients(document, ingredientsPattern);
        List<InstructionDTO> instructions = BuildRecipeInstructions(document, instructionsPattern);

        var returnDto = new DraftRecipeDTO
        {
            RecipeSummary = recipe,
            RecipeTags = tags,
            IngredientsString = ingredients,
            Instructions = instructions,
        };

        return returnDto;

        // return string.Join("\n", ingredients);

        // string recipeString = $"Title: {recipe.Name} \n Description: {recipe.Description} \n Prep Time: {recipe.PrepTime} \n Cook Time: {recipe.CookTime} \n Servings: {recipe.Servings}";
        // return recipeString;

        // List<string> insDraft = [];
        // foreach (var i in instructions)
        // {
        //     var step = $"{i.StepNumber}. {i.InstructionText}";
        //     insDraft.Add(step);
        // }
        // return string.Join("\n", insDraft);

        // List<string> ingDraft = [];
        // foreach (var i in ingredients)
        // {
        //     var step = $"{i.Quantity} {i.UnitName} {i.IngredientName}, {i.Qualifier}";
        //     ingDraft.Add(step);
        // }
        // return string.Join("\n", ingDraft);

        // return $"Recipe Type: {tags.Recipe_type} \nCuisine: {tags.Cuisine}\nMeal: {tags.Meal}\nDietary: {string.Join(", ", tags.Dietary)}";
    }
    #endregion


    #region  Get HTML
    // function to get the HTML content using AngleSharp
    public async Task<IDocument> GetHtmlAsync(string url)
    {
        // use default configuration for AngleSharp
        var config = Configuration.Default.WithDefaultLoader();
        // create a new browsing context
        var context = BrowsingContext.New(config);
        // open the given URL
        var document = await context.OpenAsync(url);
        // return IDocument
        return document;
    }
    #endregion


    #region Sample Classes
    // sample HTML elements to see if one of the popular CMS class sets is used
    public string SampleCssClasses(IDocument document)
    {
        // set the options for return
        string[] classDistinct = ["wprm", "tasty", "mv-create", "none"];

        // get a list of the CSS class names in the document
        var classNames = document.All.SelectMany(e => e.ClassList).Distinct().ToList();
        if (classNames.Count == 0)
            return "none";
        // match the classNames to established patterns in switch expression
        string patternMatch = classNames switch
        {
            var c when c.Any(x => x.StartsWith("wprm-recipe-")) => classDistinct[0],
            var c when c.Any(x => x.StartsWith("tasty-recipes-")) => classDistinct[1],
            var c when c.Any(x => x.StartsWith("mv-create-")) => classDistinct[2],
            _ => "none",
        };

        // return the matching pattern
        return patternMatch;
    }
    #endregion


    #region  Map Class Patterns
    // mapping of each CMS class set to recipe data
    public Dictionary<string, string?>? MapCssClassPatterns(string pattern)
    {
        string titlePattern;
        string descriptionPattern;
        string prepTimePattern;
        string cookTimePattern;
        string servingsPattern;
        string ingredientsPattern;
        string instructionsPattern;
        string cuisinePattern;
        string coursePattern;

        var htmlClassMap = new Dictionary<string, List<string>>
        { // lists are in order wprm, tasty, mv-create
            {
                "title",
                new List<string> { "wprm-recipe-name", "tasty-recipes-title", "mv-create-title " }
            },
            {
                "description",
                new List<string>
                {
                    "wprm-recipe-summary",
                    "tasty-recipes-description-body",
                    "mv-create-description",
                }
            },
            // Tasty: whole description (incl. header) in "tasty-recipes-description"
            {
                "prepTime",
                new List<string>
                {
                    "wprm-recipe-prep_time",
                    "tasty-recipes-prep-time",
                    "mv-create-time-prep",
                }
            },
            {
                "cookTime",
                new List<string>
                {
                    "wprm-recipe-cook_time",
                    "tasty-recipes-cook-time",
                    "mv-create-time-active",
                }
            },
            {
                "servings",
                new List<string>
                {
                    "wprm-recipe-servings ",
                    "tasty-recipes-yield",
                    "mv-create-yield",
                }
            },
            {
                "ingParent",
                new List<string>
                {
                    "wprm-recipe-ingredient",
                    "tasty-recipes-ingredients-body",
                    "mv-create-ingredients",
                }
            },
            // parent elements:
            // "wprm-recipe-ingredients-container"
            // "tasty-recipes-ingredients-body"
            // child elements:
            // <li class="wprm-recipe-ingredient"> - these also have identifying elements to make parsing easier, but would probably be a separate function
            // tasty-recipe-ingredients-body -> <li class="ingredient">
            // <li> below <h2 class="mv-create-ingredients-title">
            {
                "insParent",
                new List<string>
                {
                    "wprm-recipe-instruction-group",
                    "tasty-recipes-instructions-body",
                    "mv-create-instructions",
                }
            },
            // child elements:
            // <li id="wprm-recipe-#####-step-0-#"> (zero-indexed)><div class="wprm-recipe-instruction-text"></div></li>
            // <li id="instruction-step-#"> (one-indexed)
            // <li id="mv_create_###_#"> (one-indexed)
            {
                "course",
                new List<string>
                {
                    "wprm-recipe-course ",
                    "tasty-recipes-category",
                    "mv-create-category",
                }
            },
            {
                "cuisine",
                new List<string>
                {
                    "wprm-recipe-cuisine ",
                    "tasty-recipes-cuisine",
                    "mv-create-cuisine",
                }
            },
            {
                "author",
                new List<string>
                {
                    "wprm-recipe-author",
                    "tasty-recipes-author-name",
                    "mv-create-copy",
                }
            },
            {
                "notes",
                new List<string> { "wprm-recipe-notes", "tasty-recipes-notes" }
            },
            // child elements:
            // <span>
            // <div class="tasty-recipes-notes-body"><p></p> || <div class="tasty-recipes-notes-body">...<li>
            // -----TBD-----
        };

        int patternMatch = pattern switch
        {
            "wprm" => 0,
            "tasty" => 1,
            "mv-create" => 2,
            "none" => 3,
            _ => 3,
        };

        if (patternMatch == 3)
        {
            return null;
        }

        // build dictionary of classes to look for

        /* Recipe Summary DTO Items */
        // recipe title
        titlePattern = htmlClassMap["title"][patternMatch];
        // recipe description
        descriptionPattern = htmlClassMap["description"][patternMatch];
        // prep time
        prepTimePattern = htmlClassMap["prepTime"][patternMatch];
        // cook time
        cookTimePattern = htmlClassMap["cookTime"][patternMatch];
        // servings
        servingsPattern = htmlClassMap["servings"][patternMatch];
        /* Ingredients List DTO Items */
        ingredientsPattern = htmlClassMap["ingParent"][patternMatch];
        /* Instructions List DTO Items */
        instructionsPattern = htmlClassMap["insParent"][patternMatch];
        /* Descriptors */
        // course
        coursePattern = htmlClassMap["course"][patternMatch];
        // cuisine
        cuisinePattern = htmlClassMap["cuisine"][patternMatch];
        // TODO
        /* Misc (for now) */
        // List<string> notesMatch = htmlClassMap["notes"];
        // List<string> authorMatch = htmlClassMap["author"];

        var patterns = new Dictionary<string, string?>
        {
            { "RecipeTitle", titlePattern },
            { "Description", descriptionPattern },
            { "PrepTime", prepTimePattern },
            { "CookTime", cookTimePattern },
            { "Servings", servingsPattern },
            { "Ingredients", ingredientsPattern },
            { "Instructions", instructionsPattern },
            { "Course", coursePattern },
            { "Cuisine", cuisinePattern },
        };

        return patterns;
    }
    #endregion


    #region Build Recipe Summary
    // build recipe summary
    public RecipeDTO BuildRecipeSummary(
        IDocument document,
        string titlePattern = "",
        string descriptionPattern = "",
        string prepTimePattern = "",
        string cookTimePattern = "",
        string servingsPattern = ""
    )
    {
        // declare variables
        string? recipeTitle = "";
        string? recipeDescription = "";
        string? recipePrep = "";
        string? recipeCook = "";
        string? recipeServings = "";
        int? bakeTemp;
        string? tempUnit;

        // extract title
        if (titlePattern != "")
        {
            recipeTitle =
                document.QuerySelector($"[class*='{titlePattern}']")?.TextContent?.Trim() ?? "";
        }
        if (recipeTitle == "")
        {
            // fallback heuristic
            recipeTitle = _fallbackHeuristics.ExtractTitle(document);
        }

        // extract description
        if (descriptionPattern != "")
        {
            recipeDescription =
                document.QuerySelector($"[class*='{descriptionPattern}']")?.TextContent.Trim()
                ?? "";
        }
        if (recipeDescription == "")
        {
            // fallback heuristic
            recipeDescription = _fallbackHeuristics.ExtractDescription(document);
        }

        // extract prep time
        if (prepTimePattern != "")
        {
            var raw =
                document.QuerySelector($"[class*='{prepTimePattern}']")?.TextContent.Trim() ?? "";
            recipePrep = Regex.Replace(raw, @"^[A-Za-z ]+:?\s*", "");
        }
        if (recipePrep == "")
        {
            // fallback heuristic
            recipePrep = _fallbackHeuristics.ExtractTimeNearLabel(document, "prep time");
        }

        // extract cook time
        if (cookTimePattern != "")
        {
            var raw =
                document.QuerySelector($"[class*='{cookTimePattern}']")?.TextContent.Trim() ?? "";
            recipeCook = Regex.Replace(raw, @"^[A-Za-z ]+:?\s*", "");
        }
        if (recipeCook == "")
        {
            // fallback heuristic
            recipeCook = _fallbackHeuristics.ExtractTimeNearLabel(document, "cook time");
        }

        // extract servings/yield
        if (servingsPattern != "")
        {
            recipeServings =
                document.QuerySelector($"[class*='{servingsPattern}']")?.TextContent.Trim() ?? "";
            string pattern = @"servings: |yield: | servings";
            if (recipeServings != null)
                recipeServings = Regex.Replace(
                    recipeServings,
                    pattern,
                    string.Empty,
                    RegexOptions.IgnoreCase
                );
        }
        if (recipeServings == "")
        {
            // fallback heuristic
            recipeServings = _fallbackHeuristics.ExtractServings(document);
        }

        // bake temp
        (bakeTemp, tempUnit) = _fallbackHeuristics.ExtractBakeTemp(document);
        _ = new RecipeDTO();
        RecipeDTO? recipeSummary;
        if (bakeTemp != null)
        {
            if (tempUnit != "F" || tempUnit != "C")
                tempUnit = "F";
            recipeSummary = new RecipeDTO
            {
                Name = recipeTitle!,
                Description = recipeDescription,
                Servings = recipeServings,
                PrepTime = recipePrep,
                CookTime = recipeCook,
                BakeTemp = bakeTemp,
                Temp_unit = tempUnit,
            };
        }
        else
        {
            recipeSummary = new RecipeDTO
            {
                Name = recipeTitle!,
                Description = recipeDescription,
                Servings = recipeServings,
                PrepTime = recipePrep,
                CookTime = recipeCook,
            };
        }

        return recipeSummary;
    }
    #endregion


    #region Build Ingredients
    // TODO
    // call logic to build ingredients - for now this just returns a string list to be displayed to the user
    public List<string> BuildRecipeIngredients(IDocument document, string ingredientsPattern = "")
    {
        var ingredients = _ingredientParseService.ExtractIngredients(document, ingredientsPattern);

        return ingredients;
    }
    #endregion


    #region Build Instructions
    // build instructions
    public List<InstructionDTO> BuildRecipeInstructions(
        IDocument document,
        string instructionsPattern = ""
    )
    {
        List<string> instructionsList = [];
        List<InstructionDTO> instructionDTOs = [];

        if (instructionsPattern != "")
        {
            var instructionsElements = document.QuerySelectorAll($"div.{instructionsPattern} li");
            foreach (var i in instructionsElements)
            {
                instructionsList.Add(i.TextContent);
            }
        }
        if (instructionsList.Count == 0)
        {
            instructionDTOs = _fallbackHeuristics.ExtractInstructions(document);
            return instructionDTOs;
        }
        ;

        int stepNumber = 1;
        foreach (var i in instructionsList)
        {
            instructionDTOs.Add(
                new InstructionDTO { StepNumber = stepNumber, InstructionText = i }
            );
            stepNumber++;
        }

        return instructionDTOs;
    }
    #endregion


    #region Build Tags
    // add descriptors
    public TagStringsDTO BuildRecipeTags(
        IDocument document,
        string coursePattern = "",
        string cuisinePattern = ""
    )
    {
        // initialize returns
        string recipe_type = "";
        string cuisine = "";
        List<string> dietary = [];
        string meal = "";

        // if there is not an established pattern, go straight to fallback
        if (coursePattern == "" || cuisinePattern == "")
        {
            var extractedTags = _fallbackHeuristics.ExtractTags(document);
            return extractedTags;
        }

        // if there is an established pattern for the course/recipe type
        if (coursePattern != "")
        {
            var recipeCourseElement = document
                .QuerySelectorAll("*")
                .Where(element =>
                    element.ClassName != null && element.ClassName.Contains($"{coursePattern}")
                )
                .FirstOrDefault();
            var recipeCourseString = recipeCourseElement?.TextContent ?? "";
            var recipeTypeList = EnumExtensions.GetEnumList<RecipeTypeTag>();
            ExtractedResult<string> bestRecipeTypeMatch = Process.ExtractOne(
                recipeCourseString,
                recipeTypeList
            );
            recipe_type = bestRecipeTypeMatch.Value;
        }

        // if there is an established pattern for the cuisine
        if (cuisinePattern != "")
        {
            var recipeCuisineElement = document
                .QuerySelectorAll("*")
                .Where(element =>
                    element.ClassName != null && element.ClassName.Contains($"{cuisinePattern}")
                )
                .FirstOrDefault();
            var recipeCuisineString = recipeCuisineElement?.TextContent ?? "";
            var cuisineList = EnumExtensions.GetEnumList<CuisineTag>();
            ExtractedResult<string> bestCuisineMatch = Process.ExtractOne(
                recipeCuisineString,
                cuisineList
            );
            cuisine = bestCuisineMatch.Value;
        }

        // meal type
        meal = _heuristicExtensions.MatchEnum<MealTag>(document);

        /*  The dietary tags extraction would be best served using natural language processing,
            and with the current fallback heuristic is wrong more than it is correct. The
            functionality will be removed for now, returning a blank list for the user to select
            applicable dietary considerations for the recipe. */
        // Access document text
        // string documentText = document.Body?.TextContent ?? string.Empty;
        // // dietary
        // dietary = _fallbackHeuristics.ExtractDietaryTags(documentText);

        // return new TagStringsDTO();
        var recipeTags = new TagStringsDTO
        {
            Recipe_type = recipe_type,
            Cuisine = cuisine,
            Meal = meal,
            // Dietary = dietary
        };

        return recipeTags;
    }
    #endregion

    private static readonly Regex WhitespaceRegex = new(
        @"\s{2,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
}
