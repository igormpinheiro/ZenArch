using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using SharedKernel.Abstractions;

namespace Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Add Persistence services
    /// </summary>
    /// <param name="services">he service collection</param>
    /// <param name="configuration"></param>
    /// <returns>he service collection</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.MigrationsAssembly(AssemblyReference.Assembly.GetName().Name);
            }));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
