using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace FunctionalTests;

public sealed class TestContainerFixture : IAsyncLifetime
{
    private MsSqlContainer? _msSqlContainer;
    
    public string ConnectionString => _msSqlContainer?.GetConnectionString() 
                                      ?? throw new InvalidOperationException("Container não foi inicializado");

    public async Task InitializeAsync()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
            .WithPassword("StrongPassword123!")
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .WithLogger(LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MsSqlContainer>())
            .Build();

        await _msSqlContainer.StartAsync();
        
        await Task.Delay(2000);
    }

    public async Task DisposeAsync()
    {
        if (_msSqlContainer != null)
        {
            await _msSqlContainer.DisposeAsync();
        }
    }
}