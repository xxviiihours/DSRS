using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.Get;

public record GetPlayerByIdCommand(Guid PlayerId) : ICommand<Result<PlayerDto>>
{

}
