using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.UserLogin;

public record UserLoginCommand(string UserName, string Password) : ICommand<Result<AuthenticateResponse>>
{
}
