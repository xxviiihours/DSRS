using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Players.Queries;

public record GetOtherPlayersCommand(string? Query) : ICommand<Result<List<PlayerDto>>>
{
}
