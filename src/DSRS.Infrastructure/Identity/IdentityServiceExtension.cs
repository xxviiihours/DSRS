using DSRS.Infrastructure.Identity.Models;
using DSRS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSRS.Infrastructure.Identity;

public static class IdentityServiceExtension
{

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, ILogger logger)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;

        }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        logger.LogInformation("{Project} Identity services registered", "DSRS.Infrastructure");
        return services;
    }
}
