using Microsoft.AspNetCore.Mvc.Testing;

namespace FunctionalTests;

public class WebApplicationFactoryFixture : IDisposable
{
    private WebApplicationFactory<Program> Factory { get; }
    public HttpClient Client { get; }

    public WebApplicationFactoryFixture()
    {
        Factory = new WebApplicationFactory<Program>();
        Client = Factory.CreateClient();
    }

    public void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }
}