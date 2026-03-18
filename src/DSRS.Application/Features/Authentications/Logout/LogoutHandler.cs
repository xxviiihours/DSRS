using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.Logout;

public class LogoutHandler(IIdentityService identityService) : ICommandHandler<LogoutCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;

    public async ValueTask<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        await _identityService.SignOutAsync();
        return Result.Success();
    }
}
