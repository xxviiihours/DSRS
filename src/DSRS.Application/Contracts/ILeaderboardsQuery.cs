using DSRS.Application.Features.Leaderboards;
using System;

namespace DSRS.Application.Contracts;

public interface ILeaderboardsQuery
{

    Task<List<PlayerLeaderboardDto>> GetTop20Players(Guid playerId);
}
