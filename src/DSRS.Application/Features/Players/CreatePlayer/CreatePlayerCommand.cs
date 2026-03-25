using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.CreatePlayer;

public record CreatePlayerCommand(string Name) : ICommand<Result<Player>>
{
}
