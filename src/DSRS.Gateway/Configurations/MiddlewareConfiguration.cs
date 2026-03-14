using DSRS.Application.Exceptions;
using DSRS.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Gateway.Configurations;

public static class MiddlewareConfiguration
{
    public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerGen(options =>
            {
                options.Path = "/swagger/v1/swagger.json";
            });
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseDefaultExceptionHandler(); // from FastEndpoints
            app.UseHsts();
        }

        // Run migrations and seed in Development or when explicitly requested via environment variable
        var shouldMigrate = app.Environment.IsDevelopment() ||
                            app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

        if (shouldMigrate)
        {
            await MigrateDatabaseAsync(app);
            await SeedDatabaseAsync(app);
        }

        app.UseExceptionMiddleware();

        app.UseHttpsRedirection();
        app.UseCors("AllowedClients");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseFastEndpoints();

        return app;
    }

    static async Task MigrateDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred migrating the DB. {exceptionMessage}", ex.Message);
            throw; // Re-throw to make startup fail if migrations fail
        }
    }

    static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Seeding database...");
            var context = services.GetRequiredService<AppDbContext>();
            await SeedData.InitializeAsync(context);
            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
            // Don't re-throw for seeding errors - it's not critical
        }
    }

    static void UseExceptionMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler(app =>
        {
            app.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerFeature>()?.Error;

                if (exception is UnauthorizedException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(exception.Message);
                }
            });
        });
    }
}
