using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Players.Get;

public class GetPlayerByIdHandler(IPlayerQuery playerQuery) : ICommandHandler<GetPlayerByIdCommand, Result<PlayerDto>>
{
    private readonly IPlayerQuery _playerQuery = playerQuery;

    public async ValueTask<Result<PlayerDto>> Handle(GetPlayerByIdCommand command, CancellationToken cancellationToken)
    {
        var player = await _playerQuery.GetPlayerByIdAsync(command.PlayerId);
        if (player == null)
            return Result<PlayerDto>.Failure(
                new Error("Player.Id.NotFound", $"Player with name {command.PlayerId} does not exists."));

        return Result<PlayerDto>.Success(player);
    }
}
