using DSRS.Application.Contracts;
using DSRS.Domain.Inventories;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Market.Sell;

public class SellItemHandler(
    IInventoryRepository inventoryRepository,
    IPlayerRepository playerRepository,
    IUnitOfWork unitOfWOrk) :
    ICommandHandler<SellItemCommand, Result<Inventory>>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWOrk = unitOfWOrk;

    public async ValueTask<Result<Inventory>> Handle(SellItemCommand command, CancellationToken cancellationToken)
    {
        var result = await _playerRepository.GetByIdWithInventories(command.PlayerId);
        if (result == null)
            return Result<Inventory>.Failure(
                new Error("Inventory.Result.Empty", "Inventory not found."));

        var inventory = result.SellItem(command.ItemId, command.Quantity);
        if (!inventory.IsSuccess)
            return Result<Inventory>.Failure(inventory.Error!);

        await _inventoryRepository.UpdateAsync(inventory.Data!);
        await _unitOfWOrk.CommitAsync(cancellationToken);

        return Result<Inventory>.Success(inventory.Data!);

    }
}
