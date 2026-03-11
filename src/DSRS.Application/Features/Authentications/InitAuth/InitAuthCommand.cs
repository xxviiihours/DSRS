using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Authentications.InitAuth;

public record InitAuthCommand : ICommand<Result<PlayerDto>>
{
}
