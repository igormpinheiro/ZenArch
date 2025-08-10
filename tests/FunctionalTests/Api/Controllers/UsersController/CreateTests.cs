using System.Net;
using Application.Models;
using Shouldly;

namespace FunctionalTests.Api.Controllers.UsersController;

public class CreateTests(TestContainerFixture testContainerFixture) : ApiTestBase(testContainerFixture)
{
    private const string _usersEndpoint = "api/users";

    [Fact]
    public async Task Success()
    {
        // Arrange
        var userRequest = new UserInputModel
        {
            Name = "Test User",
            Email = "test@email.com"
        };

        // Act
        var response = await DoPostAsync(_usersEndpoint, userRequest);

        // Assert
        var createdUserData = await AssertSuccessResponseAndGetDataAsync(response, HttpStatusCode.Created);
        createdUserData.ShouldNotBeNull();
        
        var userId = createdUserData?.GetProperty("id").GetString();
        userId.ShouldNotBeNullOrWhiteSpace();
        await VerifyUserExistsAsync(userId!, userRequest.Name, userRequest.Email);
    }

    [Fact]
    public async Task Failure()
    {
        // Arrange
        var userRequest = new UserInputModel
        {
            Name = "Test User",
            Email = "duplicate@email.com"
        };

        // Create the first user
        var firstResponse = await DoPostAsync(_usersEndpoint, userRequest);
        await AssertSuccessResponseAndGetDataAsync(firstResponse, HttpStatusCode.Created);

        // Act - Attempt to create user with same email
        var duplicateResponse = await DoPostAsync(_usersEndpoint, userRequest);

        // Assert
        await AssertErrorResponseAndGetDataAsync(duplicateResponse, HttpStatusCode.Conflict);
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