using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.Queries;

public record GetPlayerByIdCommand(Guid PlayerId) : ICommand<Result<PlayerDto>>
{

}
