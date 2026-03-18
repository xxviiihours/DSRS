using DSRS.Gateway.Factories;
using DSRS.Infrastructure.Constants;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace DSRS.Gateway.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, ILogger logger)
    {
        logger.LogInformation("Configuring authentication schemes");

        services.ConfigureApplicationCookie(options => options.ApplyDefaultCookieOptions());

        return services;
    }
}
