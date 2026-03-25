using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.Queries;

public record GetPlayerByNameCommand(string Name) : ICommand<Result<Player>> { }
