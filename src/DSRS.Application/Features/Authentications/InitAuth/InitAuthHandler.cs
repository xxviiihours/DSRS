using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.InitAuth;

public class InitAuthHandler(ICurrentUserService currentUserService, IPlayerQuery playerQuery) : ICommandHandler<InitAuthCommand, Result<PlayerDto>>
{
    private readonly IPlayerQuery _playerQuery = playerQuery;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async ValueTask<Result<PlayerDto>> Handle(InitAuthCommand command, CancellationToken cancellationToken)
    {
        var player = await _playerQuery.GetPlayerByIdAsync(_currentUserService.Id);

        if (player == null)
            return Result<PlayerDto>.Failure(new Error("Player.Claim.NotFound.", "Player not found"));

        return Result<PlayerDto>.Success(player);
    }
}
