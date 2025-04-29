using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Web.API.Models;

namespace FunctionalTests.Api.Controllers;

public class UsersControllerTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    // private readonly string _connectionString = "Server=localhost,1433;Database=ZenArchDB;User Id=sa;Password=Strong.ZenPass@987;TrustServerCertificate=True;";

    public UsersControllerTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CriarUsuario_ComDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var request = new UserInputModel("test@test.com", "test");

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
        var request = new UserInputModel("test", "");

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
