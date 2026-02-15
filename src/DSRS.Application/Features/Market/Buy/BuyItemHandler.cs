using DSRS.Application.Contracts;
using DSRS.Domain.Inventories;
using DSRS.Domain.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Market.Buy;

public class BuyItemHandler(IDistributionRepository distributionRepository,
    IUnitOfWork unitOfWOrk, IPlayerRepository playerRepository) : ICommandHandler<BuyItemCommand, Result<Inventory>>
{
    private readonly IDistributionRepository _distributionRepository = distributionRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWOrk = unitOfWOrk;

    public async ValueTask<Result<Inventory>> Handle(BuyItemCommand command, CancellationToken cancellationToken)
    {

        var player = await _playerRepository.GetByIdWithDailyPrices(command.PlayerId);

        if (player == null)
            return Result<Inventory>.Failure(
                new Error("Inventory.Player.NotFound", "Player not found."));

        var result = player.BuyItem(command.ItemId, command.Quantity);

        if (!result.IsSuccess)
            return Result<Inventory>.Failure(result.Error!);

        if (result.Data!.IsNew)
            await _distributionRepository.CreateAsync(result.Data.Inventory!);
        else
            await _distributionRepository.UpdateAsync(result.Data.Inventory!);

        await _unitOfWOrk.CommitAsync(cancellationToken);

        return Result<Inventory>.Success(result.Data.Inventory!);
    }
}
