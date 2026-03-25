using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.Queries;

public record GetPlayersCommand(string? Query) : ICommand<Result<List<PlayerDto>>> { }
