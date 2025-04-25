using Domain.Interfaces.Services;
using Infrastructure.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Add Infrastructure services
    /// </summary>
    /// <param name="services">he service collection</param>
    /// <returns>he service collection</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
