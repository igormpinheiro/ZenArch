using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Interceptors;

namespace IntegrationTests;

public class IntegrationTestAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureTestServices(services =>
            {
                var descriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }
                
                services.AddScoped<AuditableEntityInterceptor>();

                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
                {
                    var interceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();

                    options.UseInMemoryDatabase("InMemoryDbForTesting").AddInterceptors(interceptor);
                    options.UseInternalServiceProvider(provider);
                    options.ConfigureWarnings(warnings =>
                        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId
                            .TransactionIgnoredWarning));
                });

                using var scope = services.BuildServiceProvider().CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.EnsureDeleted();

                // StartDatabase(dbContext);
            });
    }
}
