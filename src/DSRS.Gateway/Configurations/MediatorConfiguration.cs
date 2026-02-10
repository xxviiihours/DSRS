using DSRS.Application;
using DSRS.Application.Items.Create;
using DSRS.Application.Players.Create;
using DSRS.Infrastructure;
using System.Reflection;

namespace DSRS.Gateway.Configurations;

public static class MediatorConfiguration
{
    // Should be called from ServiceConfigs.cs, not Program.cs
    public static IServiceCollection AddMediatorSourceGen(this IServiceCollection services, ILogger logger)
    {
        logger.LogInformation("Registering Mediator SourceGen and Behaviors");
        services.AddMediator(options =>
        {
            // Lifetime: Singleton is fastest per docs; Scoped/Transient also supported.
            options.ServiceLifetime = ServiceLifetime.Scoped;

            // Supply any TYPE from each assembly you want scanned (the generator finds the assembly from the type)
            options.Assemblies =
            [
                //typeof(Player),

                // application/use case
                typeof(ApplicationAssemblyMarker), 

                // infrastructure
                typeof(InfrastructureAssemblyMarker),

                // gateway/web
                typeof(MediatorConfiguration)
            ];
        });

        return services;
    }
}