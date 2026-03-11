using DSRS.Application.Contracts;
using DSRS.Infrastructure.Identity;
using DSRS.Infrastructure.Identity.Models;
using DSRS.Infrastructure.Identity.Services;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Persistence.Migrations.Sqlite;
using DSRS.Infrastructure.Persistence.Migrations.SqlServer;
using DSRS.Infrastructure.Persistence.Queries;
using DSRS.Infrastructure.Persistence.Repositories;
using DSRS.Infrastructure.Persistence.Services;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        services.AddIdentityServices(logger);

        services.AddSingleton<IDateTime, DateTimeService>();

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IDailyPriceRepository, DailyPriceRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IDistributionHistory, DistributionHistoryRepository>();
        services.AddScoped<ISocialRepository, SocialRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

        // Event Publishing Service
        services.AddScoped<IDomainEventService, DomainEventService>();

        // Query
        services.AddScoped<ILeaderboardsQuery, LeaderboardsQuery>();
        services.AddScoped<IPlayerQuery, PlayerQuery>();
        services.AddScoped<IDashboardQuery, DashboardQuery>();

        services.AddScoped<IIdentityService, IdentityService>();

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}