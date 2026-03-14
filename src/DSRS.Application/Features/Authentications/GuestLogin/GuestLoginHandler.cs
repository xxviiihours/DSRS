using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.GuestLogin;

public class GuestLoginHandler(IPlayerRepository playerRepository, 
    IUnitOfWork unitOfWork) : ICommandHandler<GuestLoginCommand, Result<PlayerDto>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<PlayerDto>> Handle(GuestLoginCommand command, CancellationToken cancellationToken)
    {
        var player = Player.CreateGuest();

        await _playerRepository.CreateAsync(player.Data!);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<PlayerDto>.Success(
            new PlayerDto
            {
                Id = player.Data!.Id,
                Name = player.Data!.Name,
                Balance = player.Data!.Balance,
                PurchaseLimit = player.Data!.PurchaseLimit,
                IsGuest = player.Data!.IsGuest,
            });

    }
}
