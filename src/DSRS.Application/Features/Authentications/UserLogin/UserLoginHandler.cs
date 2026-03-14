using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Authentications.UserLogin;

public class UserLoginHandler(IIdentityService identityService) : ICommandHandler<UserLoginCommand, Result<AuthenticateResponse>>
{
    private readonly IIdentityService _identityService = identityService;

    public async ValueTask<Result<AuthenticateResponse>> Handle(UserLoginCommand command, CancellationToken cancellationToken)
    {
        var player = await _identityService.Authenticate(command.UserName, command.Password);
        if(player == null)
            return Result<AuthenticateResponse>.Failure(
                new Error("Authentication failed", "Invalid username or password"));

        return Result<AuthenticateResponse>.Success(
            new AuthenticateResponse(player, true));
    }
}
