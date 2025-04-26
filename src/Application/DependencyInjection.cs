using Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));

            //config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            //config.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
        });

        //services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}