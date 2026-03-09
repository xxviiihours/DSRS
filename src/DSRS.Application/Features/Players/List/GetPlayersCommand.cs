using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.List;

public record GetPlayersCommand(string? Query) : ICommand<Result<List<PlayerDto>>> { }
