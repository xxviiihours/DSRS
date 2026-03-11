namespace DSRS.Gateway.Configurations;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services, ILogger logger)
    {
        logger.LogInformation("Configuring authorization policies");
        services.AddAuthorization(options =>
        {
            options.AddPolicy("authenticated", policy =>
            {
                policy.RequireAuthenticatedUser();
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
