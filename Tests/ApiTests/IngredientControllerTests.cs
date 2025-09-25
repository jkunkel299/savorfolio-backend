using Moq;
using savorfolio_backend.Interfaces;
using FluentAssertions;
using Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Tests.ApiTests;

public class IngredientControllerTests()
{
    [Fact]
    public async Task IngredientControllerTestTerm()
    {
        // initialize search term
        string searchTerm = "chicken";

        // mock ingredient service interface
        var mockDependency = new Mock<IIngredientService>();

        // call endpoint
        _ = await IngredientEndpointsHelper.InvokeSearchEndpoint(searchTerm, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchIngredientsAsync(searchTerm), Times.Once);
    }

    [Fact]
    public async Task IngredientControllerTestNull()
    {
        // initialize search term
        string searchTerm = "";

        // initialize expected result
        string expectedString = "Search term is required.";

        // mock ingredient service interface
        var mockDependency = new Mock<IIngredientService>();

        // call endpoint
        IResult result = await IngredientEndpointsHelper.InvokeSearchEndpoint(searchTerm, mockDependency.Object);

        // check result is BadRequest<string>
        Assert.IsType<BadRequest<string>>(result);

        // cast result to BadRequest<string> to access the Value property
        var badResultValue = (BadRequest<string>)result;
        string? content = badResultValue.Value;

        // cast result to IStatusCodeHttpResult to access the StatusCode property
        var badRequestCode = result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        // assert value of BadRequest
        Assert.Equal(expectedString, content);

        // assert status code value of BadRequest
        badRequestCode.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}