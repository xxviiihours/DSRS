using DSRS.Domain.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.Get;

public record GetPlayerByNameCommand(string Name) : ICommand<Result<Player>> { }
