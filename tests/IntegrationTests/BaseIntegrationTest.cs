using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;

namespace IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected ISender Sender { get; }

    protected ApplicationDbContext DbContext { get; }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
