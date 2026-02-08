using DSRS.Application.Players.Create;
using DSRS.Infrastructure;

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
                //typeof(Player),                       // Core
                typeof(CreatePlayerCommand),         // UseCases
                typeof(InfrastuctureServiceExtensions), // Infrastructure
                typeof(MediatorConfiguration)                  // Web
            ];
        });

        return services;
    }
}