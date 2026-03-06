using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Interfaces;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Players.Get;

public class GetPlayerByNameHandler(IPlayerRepository playerRepository,
    IDateTime datetimeService, IUnitOfWork unitOfWork) : ICommandHandler<GetPlayerByNameCommand, Result<Player>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IDateTime _datetimeService = datetimeService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<Player>> Handle(GetPlayerByNameCommand command, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByName(command.Name);
        if (player == null)
            return Result<Player>.Failure(
                new Error("Player.Name.NotFound", $"Player with name {command.Name} does not exists."));

        var today = DateOnly.FromDateTime(_datetimeService.Now);

        PlayerPurchaseService.GenerateDailyPurchaseLimit(player, today);

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result<Player>.Success(player);
    }
}
