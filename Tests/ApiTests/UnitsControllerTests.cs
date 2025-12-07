using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using savorfolio_backend.Interfaces;
using Tests.Helpers;

namespace Tests.ApiTests;

public class UnitsControllerTests()
{
    [Fact]
    public async Task UnitsControllerTestTerm()
    {
        // initialize search term
        string searchTerm = "ta";

        // mock Units service interface
        var mockDependency = new Mock<IUnitsService>();

        // call endpoint
        _ = await UnitsEndpointsHelper.InvokeSearchEndpoint(searchTerm, mockDependency.Object);

        // assert mocked function called once
        mockDependency.Verify(d => d.SearchUnitsAsync(searchTerm), Times.Once);
    }

    [Fact]
    public async Task UnitsControllerTestNull()
    {
        // initialize search term
        string searchTerm = "";

        // initialize expected result
        string expectedString = "Search term is required.";

        // mock Units service interface
        var mockDependency = new Mock<IUnitsService>();

        // call endpoint
        IResult result = await UnitsEndpointsHelper.InvokeSearchEndpoint(
            searchTerm,
            mockDependency.Object
        );

        // check result is BadRequest<string>
        Assert.IsType<BadRequest<string>>(result);

        // cast result to BadRequest<string> to access the Value property
        var badResultValue = (BadRequest<string>)result;
        // get content/value from HTTP result
        string? content = badResultValue.Value;

        // cast result to IStatusCodeHttpResult to access the StatusCode property
        var badRequestCode = result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;

        // assert value of BadRequest
        Assert.Equal(expectedString, content);

        // assert status code value of BadRequest
        badRequestCode.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
