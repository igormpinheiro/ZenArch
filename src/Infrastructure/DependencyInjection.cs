using Domain.Interfaces.Services;
using Infrastructure.Identity.Services;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using SharedKernel.Abstractions;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Add Infrastructure services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration"></param>
    /// <returns>he service collection</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!)
            .AddDbContextCheck<ApplicationDbContext>();
        
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
