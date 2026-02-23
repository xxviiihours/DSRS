using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Players.Get;

public class GetPlayerByNameHandler(IPlayerQuery playerQuery) : ICommandHandler<GetPlayerByNameCommand, Result<PlayerDto>>
{
    private readonly IPlayerQuery _playerQuery = playerQuery;

    public async ValueTask<Result<PlayerDto>> Handle(GetPlayerByNameCommand command, CancellationToken cancellationToken)
    {
        var player = await _playerQuery.GetPlayerByName(command.Name);
        if (player == null) 
            return Result<PlayerDto>.Failure(
                new Error("Player.Name.NotFound", $"Player with name {command.Name} does not exists."));

        return Result<PlayerDto>.Success(player);
    }
}
