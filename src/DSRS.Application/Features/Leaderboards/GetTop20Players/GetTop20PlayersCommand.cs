using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Leaderboards.GetTop20Players;

public record GetTop20PlayersCommand(Guid PlayerId) : ICommand<Result<List<PlayerLeaderboardDto>>> { }
