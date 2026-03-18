using DSRS.Infrastructure.Constants;
using Microsoft.AspNetCore.Identity;

namespace DSRS.Gateway.Configurations;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services, ILogger logger)
    {
        logger.LogInformation("Configuring authorization policies");
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Authenticated", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
            options.AddPolicy("RegisteredUser", policy =>
            {
                policy.RequireAuthenticatedUser()
                    .RequireClaim(AppClaimTypes.IsGuest, "False");
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowedClients",
                policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173") // React app URL
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }


}
