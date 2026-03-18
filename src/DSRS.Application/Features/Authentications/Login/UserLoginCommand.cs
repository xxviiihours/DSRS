using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Authentications.Login;

public record UserLoginCommand(string UserName, string Password) : ICommand<Result<AuthenticateResponse>>
{
}
