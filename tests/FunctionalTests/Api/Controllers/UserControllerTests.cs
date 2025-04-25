using System.Net;
using System.Net.Http.Json;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FunctionalTests.Api.Controllers;

public class UserControllerTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    // private readonly string _connectionString = "Server=localhost,1433;Database=ZenArchDB;User Id=sa;Password=Strong.ZenPass@987;TrustServerCertificate=True;";

    public UserControllerTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task CriarProduto_ComDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var newUser = new User(Guid.NewGuid(), "test@test.com", "test");
        
        // Act
        var response = await _client.PostAsJsonAsync("/user", newUser);
            
        // Assert
        Assert.Equal( HttpStatusCode.Created, response.StatusCode);
        
        var userCreated = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userCreated);
        Assert.Equal(newUser.Id, userCreated.Id);
        Assert.Equal(newUser.Email, userCreated.Email);
            
        // Verifica se o produto foi realmente salvo no banco
        var getResponse = await _client.GetAsync($"/user/{userCreated.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            
        var userReturned = await getResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(userReturned);
        Assert.Equal(newUser.Id, userReturned.Id);
    }
}
