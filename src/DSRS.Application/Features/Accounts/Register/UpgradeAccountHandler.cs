using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Mappings;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Accounts.Register;

public class UpgradeAccountHandler(IPlayerRepository playerRepository,
    IIdentityService identityService, IUnitOfWork unitOfWork) : ICommandHandler<UpgradeAccountCommand, Result<PlayerDto>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<PlayerDto>> Handle(UpgradeAccountCommand command, CancellationToken cancellationToken)
    {
        var nameExists = await _playerRepository.NameExistsAsync(command.Name);
        if (nameExists)
            return Result<PlayerDto>.Failure(
                new Error("Player.Name.Exists", "Name already exists"));

        var guestPlayer = await _playerRepository.FindGuestById(command.Id);
        if (guestPlayer == null)
            return Result<PlayerDto>.Failure(
                new Error("Player.Guest.NotFound", "Guest player not found"));

        guestPlayer.UpdateName(command.Name);
        guestPlayer.UpgradeAccount();

        await _playerRepository.PatchAsync(guestPlayer);
        await _identityService.RegisterAccount(guestPlayer, command.Email, command.Password);
        await _unitOfWork.CommitAsync(cancellationToken);

        var mappedPlayer = GenericMapper.Map<Player, PlayerDto>(guestPlayer);
        return Result<PlayerDto>.Success(mappedPlayer);

    }
}
