using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.InitAuth;

public class InitAuthHandler(ICurrentUserService currentUserService, IPlayerQuery playerQuery) : ICommandHandler<InitAuthCommand, Result<AuthenticateResponse>>
{
    private readonly IPlayerQuery _playerQuery = playerQuery;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async ValueTask<Result<AuthenticateResponse>> Handle(InitAuthCommand command, CancellationToken cancellationToken)
    {
        var player = await _playerQuery.GetPlayerByIdAsync(_currentUserService.Id);

        if (player == null)
            return Result<AuthenticateResponse>.Failure(new Error("Player.Claim.NotFound.", "Player not found"));

        return Result<AuthenticateResponse>.Success(
            new AuthenticateResponse
        {
            Id = player.Id,
            UserName = player.Name
        });
    }
}
