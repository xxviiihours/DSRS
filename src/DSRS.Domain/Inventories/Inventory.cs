using DSRS.Domain.Common;
using DSRS.Domain.Items;
using DSRS.Domain.Players;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Inventories;

public class Inventory : EntityBase<Guid>, IAuditableEntity
{

    public Guid PlayerId { get; }
    public Guid ItemId { get; }
    public Item Item { get; } = null!;
    public int Quantity { get; private set; }
    public decimal PriceTotal { get; }
    public DistributionType DistributionType { get; }

    public DateTime CreatedAt { get; private set; }
    public DateTime LastModified { get; private set; }

    private Inventory() { }
    internal Inventory(
        Guid playerId,
        Guid itemId,
        int quantity,
        decimal priceTotal,
        DistributionType type)
    {
        PlayerId = playerId;
        ItemId = itemId;
        Quantity = quantity;
        PriceTotal = priceTotal;
        DistributionType = type;
    }

    public static Result<Inventory> Create(
        Guid playerId,
        Guid itemId,
        int quantity,
        decimal priceTotal,
        DistributionType type)
    {
        if (playerId == Guid.Empty)
            return Result<Inventory>.Failure(
                new Error("Inventory.PlayerId.Null", "PlayerId cannot be empty."));

        if (itemId == Guid.Empty)
            return Result<Inventory>.Failure(
                new Error("Inventory.ItemId.Null", "ItemId cannot be empty."));


        return Result<Inventory>.Success(
            new Inventory(
                playerId,
                itemId,
                quantity,
                priceTotal,
                type));
    }
    public Result AddQuantity(int amount)
    {
        if (amount <= 0)
            return Result.Failure(new Error("inventory.Quantity.Invalid", "Invalid amount value."));

        Quantity += amount;

        return Result.Success();
    }

    public void SetCreated(DateTime now)
    {
        CreatedAt = now;
        LastModified = now;
    }

    public void SetModified(DateTime now)
    {
        LastModified = now;
    }
}
