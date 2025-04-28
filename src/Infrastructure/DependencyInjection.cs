using Domain.Interfaces.Services;
using Infrastructure.Identity.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;

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
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
