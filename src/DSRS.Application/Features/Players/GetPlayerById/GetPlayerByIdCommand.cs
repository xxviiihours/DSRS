using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.GetPlayerById;

public record GetPlayerByIdCommand(Guid PlayerId) : ICommand<Result<PlayerDto>>
{

}
