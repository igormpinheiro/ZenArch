using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace FunctionalTests.Api.Controllers;

public class UsersControllerTests
{
    private readonly HttpClient _client;
    public UsersControllerTests()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarUsuario_ComDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var request = new UserInputModel
        {
            Name = "test",
            Email = "test@test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/users", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("isSuccess").GetBoolean().ShouldBe(true);
        responseData.RootElement.GetProperty("data").GetProperty("name").GetString().ShouldSatisfyAllConditions(
            name => name.ShouldNotBeNullOrWhiteSpace(),
            name => name.ShouldBe(request.Name));
        responseData.RootElement.GetProperty("data").GetProperty("email").GetString().ShouldSatisfyAllConditions(
            email => email.ShouldNotBeNullOrWhiteSpace(),
            email => email.ShouldBe(request.Email));

        // Verifica se o usuario foi realmente salvo no banco
        var userId = responseData.RootElement.GetProperty("data").GetProperty("id").GetString();
        var getResponse = await _client.GetAsync($"api/users/{userId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var getResponseBody = await getResponse.Content.ReadAsStreamAsync();
        var getResponseData = await JsonDocument.ParseAsync(getResponseBody);
        getResponseData.RootElement.GetProperty("data").GetProperty("id").GetString().ShouldSatisfyAllConditions(
            id => id.ShouldNotBeNullOrWhiteSpace(),
            id => id.ShouldBe(userId));
    }
    
    [Fact]
    public async Task CriarUsuario_ComDadosInvalidos_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new UserInputModel
        {
            Name = "",
            Email = "test"
        };
        // Act
        var response = await _client.PostAsJsonAsync("api/users", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("isSuccess").GetBoolean().ShouldBe(false);
        responseData.RootElement.GetProperty("error").GetProperty("validationErrors").GetArrayLength().ShouldBe(2);
    }
}
