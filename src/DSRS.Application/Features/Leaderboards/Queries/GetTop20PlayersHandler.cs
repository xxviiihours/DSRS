using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System.Data.Common;

namespace DSRS.Application.Features.Leaderboards.Queries;

public class GetTop20PlayersHandler(ILeaderboardsQuery leaderboardQuery) :
    ICommandHandler<GetTop20PlayersCommand, Result<List<PlayerLeaderboardDto>>>
{
    private readonly ILeaderboardsQuery _leaderboardQuery = leaderboardQuery;

    public async ValueTask<Result<List<PlayerLeaderboardDto>>> Handle(
        GetTop20PlayersCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaderboardQuery.GetTop20Players(command.PlayerId);
            if (result == null)
                return Result<List<PlayerLeaderboardDto>>.Failure(
                    new Error("Leaderboards.Empty", "No top players found."));

            return Result<List<PlayerLeaderboardDto>>.Success(result);

        }
        catch (DbException ex)
        {
            return Result<List<PlayerLeaderboardDto>>.Failure(
                new Error("Database.Error", $"Database error {ex.Message}"));
        }
    }
}
