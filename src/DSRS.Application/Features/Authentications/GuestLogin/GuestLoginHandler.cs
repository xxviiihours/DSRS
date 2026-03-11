using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.GuestLogin;

public class GuestLoginHandler(IPlayerRepository playerRepository, IUnitOfWork unitOfWork) : ICommandHandler<GuestLoginCommand, Result<AuthenticateResponse>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<AuthenticateResponse>> Handle(GuestLoginCommand command, CancellationToken cancellationToken)
    {
        var player = Player.CreateGuest();

        await _playerRepository.CreateAsync(player.Data!);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<AuthenticateResponse>.Success(
            new AuthenticateResponse(player.Data!.Id, player.Data!.Name));

    }
}
