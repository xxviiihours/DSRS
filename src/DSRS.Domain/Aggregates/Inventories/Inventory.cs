using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Common;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Aggregates.Inventories;

public class Inventory : AggregateRoot<InventoryId>, IAuditableEntity
{

    public PlayerId PlayerId { get; }
    public ItemId ItemId { get; }
    public Item Item { get; } = null!;
    public int Quantity { get; private set; }
    public Money PurchasePrice { get; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastModified { get; private set; }

    //private Inventory() { }
    internal Inventory(
        PlayerId playerId,
        ItemId itemId,
        int quantity,
        Money purchasePrice)
    {
        Id = InventoryId.New();
        PlayerId = playerId;
        ItemId = itemId;
        Quantity = quantity;
        PurchasePrice = purchasePrice;
    }

    public static Result<Inventory> Create(
        PlayerId playerId,
        ItemId itemId,
        int quantity,
        Money purchasePrice)
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
                purchasePrice));
    }
    public Result Increase(int amount)
    {
        if (amount <= 0)
            return Result.Failure(
                new Error("inventory.Amount.Invalid", "Invalid amount value."));

        Quantity += amount;

        return Result.Success();
    }

    public Result Decrease(int amount)
    {
        if (amount <= 0)
            return Result.Failure(
                new Error("inventory.Amount.Invalid", "Invalid amount value."));

        Quantity -= amount;

        return Result.Success();
    }

    public bool HasEnough(int amount) => Quantity >= amount;

    public bool IsQuantityExceeded(int amount) => amount > Quantity;

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
