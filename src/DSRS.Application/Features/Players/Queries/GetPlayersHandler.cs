using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.Queries;

public class GetPlayersHandler(IPlayerQuery playerQuery) : ICommandHandler<GetPlayersCommand, Result<List<PlayerDto>>>
{
  private readonly IPlayerQuery _playerQuery = playerQuery;

  public async ValueTask<Result<List<PlayerDto>>> Handle(GetPlayersCommand command, CancellationToken cancellationToken)
  {
    var players = await _playerQuery.GetPlayers(command.Query!);
    if (players.Count < 0)
      return Result<List<PlayerDto>>.Failure(
        new Error("Player.List.Empty", "Empty player list"));


    return Result<List<PlayerDto>>.Success(players);
  }
}
