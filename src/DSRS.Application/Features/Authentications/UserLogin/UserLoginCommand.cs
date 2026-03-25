using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Authentications.UserLogin;

public record UserLoginCommand(string UserName, string Password) : ICommand<Result<AuthenticateResponse>>
{
}
