using System.Net;
using Application.Models;
using Shouldly;

namespace FunctionalTests.Api.Controllers.UsersController;

public class DeleteTests(TestContainerFixture testContainerFixture) : ApiTestBase(testContainerFixture)
{
    private const string _usersEndpoint = "api/users";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var createRequest = new UserInputModel
        {
            Name = "User To Delete",
            Email = "delete@email.com"
        };

        var createResponse = await DoPostAsync(_usersEndpoint, createRequest);
        var createdUserData = await AssertSuccessResponseAndGetDataAsync(createResponse, HttpStatusCode.Created);
        var userId = createdUserData?.GetProperty("id").GetString();
        userId.ShouldNotBeNullOrWhiteSpace();

        // Act
        var response = await DoDeleteAsync($"{_usersEndpoint}/{userId}");

        // Assert
        await AssertSuccessResponseAndGetDataAsync(response, HttpStatusCode.NoContent);
        await VerifyUserDoesNotExistAsync(userId!);
    }

    [Fact]
    public async Task Failure()
    {
        var invalidId = new Guid("00000000-0000-0000-0000-000000000001");

        // Act - Attempt to delete non-existing user
        var response = await DoDeleteAsync($"{_usersEndpoint}/{invalidId}");

        // Assert
        await AssertErrorResponseAndGetDataAsync(response, HttpStatusCode.NotFound);
    }

    private async Task VerifyUserDoesNotExistAsync(string userId)
    {
        var getUserResponse = await DoGetAsync($"{_usersEndpoint}/{userId}");
        await AssertErrorResponseAndGetDataAsync(getUserResponse, HttpStatusCode.NotFound);
    }
}
