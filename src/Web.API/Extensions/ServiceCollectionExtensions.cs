using System.Text.Json;
using System.Text.Json.Serialization;
using Web.API.Middlewares;

namespace Web.API.Extensions;

/// <summary>
/// Extension methods for service collection
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all API services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<ErrorHandlingMiddleware>();
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.WriteIndented = false;
        });
        services.AddOpenApi();
        
        services.AddRouting(options => options.LowercaseUrls = true);
        
        return services;
    }
}
