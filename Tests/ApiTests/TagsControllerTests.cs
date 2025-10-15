using Newtonsoft.Json;
using Tests.Helpers;

namespace Tests.ApiTests;

public class TagsControllerTests()
{
    private static readonly string[] _expectedMealTags;
    private static readonly string[] _expectedRecipeTypeTags;
    private static readonly string[] _expectedCuisineTags;
    private static readonly List<string> _expectedDietaryTags;

    static TagsControllerTests()
    {
        string mealTagsFilePath = TestFileHelper.GetProjectPath("ExpectedData/Enums/MealTypeEnum.json");
        string recipeTypeTagsFilePath = TestFileHelper.GetProjectPath("ExpectedData/Enums/RecipeTypeEnum.json");
        string cuisineTagsFilePath = TestFileHelper.GetProjectPath("ExpectedData/Enums/CuisineEnum.json");
        string dietaryTagsFilePath = TestFileHelper.GetProjectPath("ExpectedData/Enums/DietaryEnum.json");


        _expectedMealTags = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(mealTagsFilePath)) ?? [];
        _expectedRecipeTypeTags = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(recipeTypeTagsFilePath)) ?? [];
        _expectedCuisineTags = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(cuisineTagsFilePath)) ?? [];
        _expectedDietaryTags = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(dietaryTagsFilePath)) ?? [];
    }



    // tests the meal tags endpoint
    [Fact]
    public void GetMealTagsTest()
    {
        // call endpoint
        var result = TagsEndpointsHelper.InvokeGetMealTagsEndpoint();

        // assert result.ok type
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<string[]>>(result);
        var actualValues = okResult.Value;

        // assert result.ok value is expected
        Assert.Equal(_expectedMealTags, actualValues);
    }



    // tests the recipe type tags endpoint
    [Fact]
    public void GetRecipeTypeTagsTest()
    {
        // call endpoint
        var result = TagsEndpointsHelper.InvokeGetRecipeTypeTagsEndpoint();

        // assert result.ok type
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<string[]>>(result);
        var actualValues = okResult.Value;

        // assert result.ok value is expected
        Assert.Equal(_expectedRecipeTypeTags, actualValues);
    }



    // tests the cuisine tags endpoint
    [Fact]
    public void GetCuisineTagsTest()
    {
        // call endpoint
        var result = TagsEndpointsHelper.InvokeGetCuisineTagsEndpoint();

        // assert result.ok type
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<string[]>>(result);
        var actualValues = okResult.Value;

        // assert result.ok value is expected
        Assert.Equal(_expectedCuisineTags, actualValues);
    }



    // tests the dietary tags endpoint
    [Fact]
    public void GetDietaryTagsTest()
    {
        // call endpoint
        var result = TagsEndpointsHelper.InvokeGetDietaryTagsEndpoint();

        // assert result.ok type
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<string>>>(result);
        var actualValues = okResult.Value;

        // assert result.ok value is expected
        Assert.Equal(_expectedDietaryTags, actualValues);
    }
}