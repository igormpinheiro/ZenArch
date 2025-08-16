using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Shouldly;

namespace FunctionalTests;

public abstract class ApiTestBase : IClassFixture<TestContainerFixture>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    protected ApiTestBase(TestContainerFixture testContainerFixture)
    {
        _factory = new CustomWebApplicationFactory(testContainerFixture);
        _client = _factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> DoPostAsync<TRequest>(string endpoint, TRequest request)
    {
        return await _client.PostAsJsonAsync(endpoint, request);
    }

    protected async Task<HttpResponseMessage> DoGetAsync(string endpoint)
    {
        return await _client.GetAsync(endpoint);
    }

    protected async Task<HttpResponseMessage> DoPutAsync<TRequest>(string endpoint, TRequest request)
    {
        return await _client.PutAsJsonAsync(endpoint, request);
    }

    protected async Task<HttpResponseMessage> DoDeleteAsync(string endpoint)
    {
        return await _client.DeleteAsync(endpoint);
    }
    protected async Task<JsonElement?> AssertSuccessResponseAndGetDataAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        response.StatusCode.ShouldBe(expectedStatusCode);
        if (expectedStatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("isSuccess").GetBoolean().ShouldBe(true);
        responseData.RootElement.TryGetProperty("data", out var dataProperty).ShouldBe(true);
        return dataProperty;
    }

    protected async Task<JsonElement?> AssertErrorResponseAndGetDataAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        response.StatusCode.ShouldBe(expectedStatusCode);
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        responseData.RootElement.GetProperty("isSuccess").GetBoolean().ShouldBe(false);
        responseData.RootElement.TryGetProperty("error", out var errorProperty).ShouldBe(true);
        return errorProperty;
    }
    
    // private static JsonSerializerOptions GetJsonOptions()
    // {
    //     return new JsonSerializerOptions
    //     {
    //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    //         PropertyNameCaseInsensitive = true
    //     };
    // }

    // protected async Task CleanupDatabaseAsync()
    // {
    //     using var scope = _factory.Services.CreateScope();
    //     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //     
    //     context.Users.RemoveRange(context.Users);
    //     await context.SaveChangesAsync();
    // }
    
    // protected async Task SeedTestDataAsync()
    // {
    //     using var scope = _factory.Services.CreateScope();
    //     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //     
    //     await context.SaveChangesAsync();
    // }

    public virtual void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}
