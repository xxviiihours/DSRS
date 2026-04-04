using DSRS.Application.Contracts;
using DSRS.Domain.ValueObjects;
using DSRS.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DSRS.Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUserService
{
    public PlayerId Id { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirstValue(AppClaimTypes.NameIdentifier);

        Id = !string.IsNullOrEmpty(claim) ? PlayerId.From(Guid.Parse(claim)) : PlayerId.Empty();
        IsAuthenticated = !string.IsNullOrEmpty(claim);
    }
}
