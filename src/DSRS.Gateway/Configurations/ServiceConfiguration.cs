using DSRS.Infrastructure;

namespace DSRS.Gateway.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddServiceConfigurations(this IServiceCollection services, ILogger logger, WebApplicationBuilder builder)
    {
        services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatorSourceGen(logger);


        logger.LogInformation("{Project} services registered", "Mediator Source Generator");

        return services;
    }
}
