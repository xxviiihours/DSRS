using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.GetOtherPlayers;

public record GetOtherPlayersCommand(string? Query) : ICommand<Result<List<PlayerDto>>>
{
}
