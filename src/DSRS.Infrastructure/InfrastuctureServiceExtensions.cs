using DSRS.Application.Interfaces;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSRS.Infrastructure;

public static class InfrastuctureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        ConfigurationManager config, ILogger logger)
    {
        string? connectionString = config.GetConnectionString("DefaultConnection") 
            ?? config.GetConnectionString("SqliteConnection");


        services.AddDbContext<AppDbContext>((provider, options) =>
        {

            if (config.GetConnectionString("cleanarchitecture") != null ||
                config.GetConnectionString("DefaultConnection") != null)
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }

        });

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}