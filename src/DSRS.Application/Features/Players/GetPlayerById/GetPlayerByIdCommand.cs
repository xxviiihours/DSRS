using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.GetPlayerById;

public record GetPlayerByIdCommand(PlayerId PlayerId) : ICommand<Result<PlayerDto>>
{

}
