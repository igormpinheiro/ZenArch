using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Interceptors;
using Persistence.Repositories;
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
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>((provider, options) =>
        {
            var auditInterceptor = provider.GetRequiredService<AuditableEntityInterceptor>();
            var eventInterceptor = provider.GetRequiredService<DomainEventInterceptor>();

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.MigrationsAssembly(AssemblyReference.Assembly.FullName);
            }).AddInterceptors(auditInterceptor, eventInterceptor);
        });
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
