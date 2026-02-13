using DSRS.Domain.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.Create;

public record CreatePlayerCommand(string Name, decimal Balance) : ICommand<Result<Player>>
{
}
