using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Mappings;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications.Login;

public class GuestLoginHandler(IPlayerRepository playerRepository,
    IUnitOfWork unitOfWork, IIdentityService identityService) : ICommandHandler<GuestLoginCommand, Result<PlayerDto>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IIdentityService _identityService = identityService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<PlayerDto>> Handle(GuestLoginCommand command, CancellationToken cancellationToken)
    {
        var player = Player.CreateGuest();

        await _playerRepository.CreateAsync(player.Data!);
        await _unitOfWork.CommitAsync(cancellationToken);

        var result = await _identityService.AuthenticateAsGuest(player.Data!);

        if(result == null)
            return Result<PlayerDto>.Failure(
                new Error("Guest.Login.Failed", "Failed to authenticate as guest."));

        var mappedPlayer = GenericMapper.Map<Player, PlayerDto>(player.Data!);

        return Result<PlayerDto>.Success(mappedPlayer);
    }
}
