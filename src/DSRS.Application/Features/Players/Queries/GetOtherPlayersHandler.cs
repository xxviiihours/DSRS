using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.Queries;

public class GetOtherPlayersHandler(IPlayerQuery playerQuery) : ICommandHandler<GetOtherPlayersCommand, Result<List<PlayerDto>>>
{
    private readonly IPlayerQuery _playerQuery = playerQuery;

    public async ValueTask<Result<List<PlayerDto>>> Handle(GetOtherPlayersCommand command, CancellationToken cancellationToken)
    {
        var otherPlayers = await _playerQuery.GetOtherPlayers(command.Query!);
        if (otherPlayers.Count < 0)
            return Result<List<PlayerDto>>.Failure(
              new Error("Player.List.Empty", "Empty player list"));

        return Result<List<PlayerDto>>.Success(otherPlayers);

    }
}
