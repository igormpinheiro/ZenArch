using System.Net;
using Application.Models;
using Shouldly;

namespace FunctionalTests.Api.Controllers.UsersController;

public class GetByIdTests(TestContainerFixture testContainerFixture) : ApiTestBase(testContainerFixture)
{
    private const string _usersEndpoint = "api/users";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var createRequest = new UserInputModel
        {
            Name = "Test User",
            Email = "test@email.com"
        };

        var createResponse = await DoPostAsync(_usersEndpoint, createRequest);
        var createdUserData = await AssertSuccessResponseAndGetDataAsync(createResponse, HttpStatusCode.Created);
        var userId = createdUserData?.GetProperty("id").GetString();
        userId.ShouldNotBeNullOrWhiteSpace();

        // Act
        var response = await DoGetAsync($"{_usersEndpoint}/{userId}");

        // Assert
        var userData = await AssertSuccessResponseAndGetDataAsync(response, HttpStatusCode.OK);

        userData?.GetProperty("id").GetString().ShouldBe(userId);
        userData?.GetProperty("name").GetString().ShouldBe(createRequest.Name);
        userData?.GetProperty("email").GetString().ShouldBe(createRequest.Email);
    }

    [Fact]
    public async Task Failure()
    {
        var invalidId = new Guid("00000000-0000-0000-0000-000000000001");

        // Act - Attempt to get non-existing user
        var response = await DoGetAsync($"{_usersEndpoint}/{invalidId}");

        // Assert
        await AssertErrorResponseAndGetDataAsync(response, HttpStatusCode.NotFound);
    }
}
