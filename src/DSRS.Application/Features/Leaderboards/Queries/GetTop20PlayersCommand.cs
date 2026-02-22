using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Windows.Input;

namespace DSRS.Application.Features.Leaderboards.Queries;

public record GetTop20PlayersCommand(Guid PlayerId) : ICommand<Result<List<PlayerLeaderboardDto>>> { }
