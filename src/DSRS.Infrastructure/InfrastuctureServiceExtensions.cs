using DSRS.Application.Contracts;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Persistence.Migrations.Sqlite;
using DSRS.Infrastructure.Persistence.Migrations.SqlServer;
using DSRS.Infrastructure.Repositories;
using DSRS.Infrastructure.Services;
using DSRS.SharedKernel.Interfaces;
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
        bool shouldUseDefaultDatabase = config.GetValue<bool>("Database:UseDefault");
        if (shouldUseDefaultDatabase)
        {
            services.AddDbContext<AppDbContext, SqlServerDbContext>((provider, options) =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("DSRS.Infrastructure")
                        .MigrationsHistoryTable("__EFMigrationsHistory", "sqlserver"));
            });
        }
        else
        {
            services.AddDbContext<AppDbContext, SqliteDbContext>((provider, options) =>
            {
                options.UseSqlite(config.GetConnectionString("SqliteConnection"),
                    x => x.MigrationsAssembly("DSRS.Infrastructure")
                        .MigrationsHistoryTable("__EFMigrationsHistory", "sqlite"));
            });
        }


        services.AddSingleton<IDateTime, DateTimeService>();

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();
        services.AddScoped<IDailyPriceRepository, DailyPriceRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}