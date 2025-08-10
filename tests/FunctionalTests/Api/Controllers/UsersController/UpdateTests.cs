using System.Net;
using Application.Models;
using Shouldly;

namespace FunctionalTests.Api.Controllers.UsersController;

public class UpdateTests(TestContainerFixture testContainerFixture) : ApiTestBase(testContainerFixture)
{
    private const string _usersEndpoint = "api/users";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var createRequest = new UserInputModel
        {
            Name = "Original User",
            Email = "original@email.com"
        };

        var createResponse = await DoPostAsync(_usersEndpoint, createRequest);
        var createdUserData = await AssertSuccessResponseAndGetDataAsync(createResponse, HttpStatusCode.Created);
        var userId = createdUserData?.GetProperty("id").GetString();
        userId.ShouldNotBeNullOrWhiteSpace();
        
        var updateRequest = new UserInputModel
        {
            Name = "Updated User",
            Email = "updated@email.com"
        };

        // Act
        var response = await DoPutAsync($"{_usersEndpoint}/{userId}", updateRequest);

        // Assert
        await AssertSuccessResponseAndGetDataAsync(response, HttpStatusCode.OK);
        await VerifyUserExistsAsync(userId!, updateRequest.Name, updateRequest.Email);
    }

    [Fact]
    public async Task Failure()
    {
        // Arrange
        var updateRequest = new UserInputModel
        {
            Name = "Updated User",
            Email = "updated@email.com"
        };
        var invalidId = new Guid("00000000-0000-0000-0000-000000000001");
        
        // Act - Attempt to update non-existing user
        var response = await DoPutAsync($"{_usersEndpoint}/{invalidId}", updateRequest);

        // Assert
        await AssertErrorResponseAndGetDataAsync(response, HttpStatusCode.NotFound);
    }

    private async Task VerifyUserExistsAsync(string userId, string expectedName, string expectedEmail)
    {
        var getUserResponse = await DoGetAsync($"{_usersEndpoint}/{userId}");
        var userData = await AssertSuccessResponseAndGetDataAsync(getUserResponse);

        userData?.GetProperty("id").GetString().ShouldBe(userId);
        userData?.GetProperty("name").GetString().ShouldBe(expectedName);
        userData?.GetProperty("email").GetString().ShouldBe(expectedEmail);
    }
}
