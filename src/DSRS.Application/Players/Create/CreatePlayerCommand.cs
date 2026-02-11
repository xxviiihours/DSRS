using DSRS.Domain.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Players.Create;

public record CreatePlayerCommand(string Name, decimal Balance) : ICommand<Result<Player>>
{
}
