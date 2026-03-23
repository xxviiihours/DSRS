using DSRS.Application.Contracts;
using DSRS.Application.Options;
using DSRS.Infrastructure.Identity;
using DSRS.Infrastructure.Identity.Services;
using DSRS.Infrastructure.Jobs;
using DSRS.Infrastructure.Persistence;
using DSRS.Infrastructure.Persistence.Migrations.Sqlite;
using DSRS.Infrastructure.Persistence.Migrations.SqlServer;
using DSRS.Infrastructure.Persistence.Queries;
using DSRS.Infrastructure.Persistence.Repositories;
using DSRS.Infrastructure.Persistence.Services;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

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
        services.AddHttpContextAccessor();

        services.AddSingleton<IDateTime, DateTimeService>();

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IDailyPriceRepository, DailyPriceRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IDistributionHistoryRepository, DistributionHistoryRepository>();
        services.AddScoped<ISocialRepository, SocialRepository>();
        services.AddScoped<IPlayerSnapshotRepository, PlayerSnapshotRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

        // Event Publishing Service
        services.AddScoped<IDomainEventService, DomainEventService>();

        // Query
        services.AddScoped<ILeaderboardsQuery, LeaderboardsQuery>();
        services.AddScoped<IPlayerQuery, PlayerQuery>();
        services.AddScoped<IDashboardQuery, DashboardQuery>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IMarketService, MarketService>();

        services.Configure<BackgroundJobOption>(config.GetSection("BackgroundJobs"));

        services.AddQuartz(q =>
        {
            var priceJobKey = new JobKey("DailyPriceJob");

            q.AddJob<GeneratePriceJob>(opts => opts.WithIdentity(priceJobKey));

            q.AddTrigger(opts => opts
                .ForJob(priceJobKey)
                .WithIdentity("DailyPriceJob-trigger")
                .WithCronSchedule("0 0 0 * * ?"));
            //.WithSimpleSchedule(sched =>
            //    sched.WithIntervalInMinutes(1)
            //    .WithRepeatCount(1)));

            var limitJobKey = new JobKey("PurchaseLimitJob");

            q.AddJob<GeneratePurchaseLimitJob>(opts => opts.WithIdentity(limitJobKey));

            q.AddTrigger(opts => opts
                .ForJob(limitJobKey)
                .WithIdentity("PurchaseLimitJob-trigger")
                .WithCronSchedule("0 0 0 * * ?"));
            //.WithSimpleSchedule(sched =>
            //        sched.WithIntervalInMinutes(1)
            //        .WithRepeatCount(1)));
        });

        services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = true;
        });

        logger.LogInformation("{Project} services registered", "Infrastructure");

        return services;
    }
}