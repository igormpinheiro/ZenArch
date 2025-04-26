namespace Web.API.Extensions;

/// <summary>
/// Extension methods for service collection
/// </summary>
public static class ApplicationServicesExtensions
{
    /// <summary>
    /// Adds all API services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}
