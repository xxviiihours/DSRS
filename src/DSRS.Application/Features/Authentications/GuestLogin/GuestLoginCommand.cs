using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Authentications.GuestLogin;

public record GuestLoginCommand : ICommand<Result<PlayerDto>>
{
}
