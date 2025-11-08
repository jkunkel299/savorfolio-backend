using Moq;
using Newtonsoft.Json.Linq;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;
using Tests.Helpers;

namespace Tests.ApiTests;

public class AddRecipeControllerTests()
{
    private static readonly JObject _expectedAddRecipe;

    static AddRecipeControllerTests()
    {
        string addRecipeFilePath = TestFileHelper.GetProjectPath("ExpectedData/AddRecipe.json");
        _expectedAddRecipe = JObject.Parse(File.ReadAllText(addRecipeFilePath));
    }


    // test for MapManualRecipe success
    [Fact]
    public async Task AddRecipeManualSuccess()
    {
        // initialize recipeId
        int recipeId = 3;
        // initialize JSON body
        var jsonBody = _expectedAddRecipe.ToString();
        // initialize expected HTTP response message
        string expectedMessage = $"Recipe ID {recipeId} added successfully";

        // mock AddRecipeService
        var mockAddRecipeService = new Mock<IAddRecipeService>();
        // set up mocked service to return an expected success
        mockAddRecipeService
            .Setup(d => d.AddRecipeManually(It.IsAny<JObject>()))
            .ReturnsAsync(new OperationResult<int>
            {
                Success = true,
                Data = recipeId,
                Message = "Recipe added successfully"
            });

        // call emulated API endpoint
        var result = await RecipeEndpointsHelper.InvokeAddManualRecipeEndpoint(jsonBody, mockAddRecipeService.Object);

        // assert AddRecipeManually called once
        mockAddRecipeService.Verify(d => d.AddRecipeManually(It.IsAny<JObject>()), Times.AtMostOnce);

        // assert result.Ok type
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<string>>(result);
        // assert result.Ok message
        Assert.Equal(expectedMessage, okResult.Value);
    }

    // test for MapManualRecipe failure
    [Fact]
    public async Task AddRecipeManualFailure()
    {
        // initialize JSON body
        var jsonBody = _expectedAddRecipe.ToString();
        // initialize the expected HTTP response message
        string expectedMessage = "Recipe not added successfully";

        // mock AddRecipeService
        var mockAddRecipeService = new Mock<IAddRecipeService>();
        // set up mocked service to return an expected failure
        mockAddRecipeService
            .Setup(d => d.AddRecipeManually(It.IsAny<JObject>()))
            .ReturnsAsync(new OperationResult<int>
            {
                Success = false,
                Message = "Failed to add recipe"
            });

        // call emulated API endpoint
        var result = await RecipeEndpointsHelper.InvokeAddManualRecipeEndpoint(jsonBody, mockAddRecipeService.Object);

        // assert AddRecipeManually called once
        mockAddRecipeService.Verify(d => d.AddRecipeManually(It.IsAny<JObject>()), Times.AtMostOnce);

        // assert result.Problem type
        var badResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        // assert result.Problem message
        Assert.Equal(expectedMessage, badResult.ProblemDetails.Detail);
    }
}