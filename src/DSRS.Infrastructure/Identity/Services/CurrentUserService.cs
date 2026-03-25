using DSRS.Application.Contracts;
using DSRS.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DSRS.Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid Id { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirstValue(AppClaimTypes.NameIdentifier);

        Id = !string.IsNullOrEmpty(claim) ? Guid.Parse(claim) : Guid.Empty;
        IsAuthenticated = !string.IsNullOrEmpty(claim);
    }
}
