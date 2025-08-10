using System.Net;
using Application.Models;
using Shouldly;

namespace FunctionalTests.Api.Controllers.UsersController;

public class GetAllTests(TestContainerFixture testContainerFixture) : ApiTestBase(testContainerFixture)
{
    private const string _usersEndpoint = "api/users";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var user1Request = new UserInputModel
        {
            Name = "User One",
            Email = "user1@email.com"
        };

        var user2Request = new UserInputModel
        {
            Name = "User Two",
            Email = "user2@email.com"
        };

        await DoPostAsync(_usersEndpoint, user1Request);
        await DoPostAsync(_usersEndpoint, user2Request);

        // Act
        var response = await DoGetAsync(_usersEndpoint);

        // Assert
        var usersArray = await AssertSuccessResponseAndGetDataAsync(response, HttpStatusCode.OK);

        usersArray?.GetArrayLength().ShouldBeGreaterThanOrEqualTo(2);
    }

    // [Fact]
    // public async Task Failure()
    // {
    //     // Act - Get users with invalid query parameters
    //     var response = await DoGetAsync($"{_usersEndpoint}?invalidParam=invalidValue&limit=-1");
    //
    //     // Assert
    //     await AssertErrorResponseAndGetDataAsync(response, HttpStatusCode.BadRequest);
    // }
}
