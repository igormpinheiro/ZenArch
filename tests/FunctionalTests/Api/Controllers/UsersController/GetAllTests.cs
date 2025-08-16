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
        var response = await DoGetAsync($"{_usersEndpoint}?page=1&pageSize=10");

        // Assert
        var usersArray = await AssertSuccessResponseAndGetDataAsync(response, HttpStatusCode.OK);

        usersArray?.GetProperty("items").GetArrayLength().ShouldBe(2);
    }

    // [Fact]
    // public async Task Failure()
    // {
    //     // Act - Get users with invalid pagination parameters
    //     var response = await DoGetAsync($"{_usersEndpoint}?page=0&pageSize=101");
    //
    //     // Assert
    //     await AssertErrorResponseAndGetDataAsync(response, HttpStatusCode.BadRequest);
    // }
}
