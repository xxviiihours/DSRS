using DSRS.Application.Contracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DSRS.Infrastructure.Identity.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid Id { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        Id = Guid.Parse(claim!);
        IsAuthenticated = !string.IsNullOrEmpty(claim);
    }
}
