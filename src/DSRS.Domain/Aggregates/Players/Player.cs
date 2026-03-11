using DSRS.Domain.Aggregates.Distributions;
using DSRS.Domain.Aggregates.Inventories;
using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.Events;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Aggregates.Players;

public sealed class Player : AggregateRoot<Guid>
{
    public string Name { get; } = string.Empty;
    public decimal Balance { get; private set; }
    public int PurchaseLimit { get; private set; }
    public bool IsGuest { get; private set; }

    private const int MaxPurchaseLimit = 100;
    public DateOnly LastLimitGeneration { get; private set; }
    private readonly List<DailyPrice> _dailyPrices = [];
    public IReadOnlyCollection<DailyPrice> DailyPrices => _dailyPrices.AsReadOnly();
    private readonly List<Inventory> _inventoryItems = [];
    public IReadOnlyCollection<Inventory> InventoryItems => _inventoryItems.AsReadOnly();

    private Player(string name, bool isGuest = false)
    {
        Name = name;
        Balance = 1000;
        IsGuest = isGuest;
        PurchaseLimit = 25;
        LastLimitGeneration = DateOnly.FromDateTime(DateTime.Now);
    }

    public static Result<Player> CreateGuest()
    {
        var name = $"Guest_{Random.Shared.Next(1000, 9999)}";
        return Result<Player>.Success(new Player(name, true));
    }

    public static Result<Player> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Player>.Failure(
                new Error("Player.Name.Empty", "Player name cannot be empty"));

        // maybe add domain event here

        return Result<Player>.Success(new Player(name));
    }
    public void Register() => IsGuest = false;
    public void RegenerateLimit(int maxLimit, int amount) => PurchaseLimit = Math.Min(PurchaseLimit + amount, maxLimit);
    public void ConsumeLimit(int amount) => PurchaseLimit -= amount;
    public bool CanAfford(decimal amount) => Balance >= amount;
    public void DecreaseBalance(decimal amount) => Balance -= amount;
    public void IncreaseBalance(decimal amount) => Balance += amount;
    public void SetLastGeneration(DateOnly date) => LastLimitGeneration = date;

    #region Daily Price
    public Result<DailyPrice> AddDailyPrice(Item item, DateOnly date,
         decimal price, decimal percentage, PriceState state)
    {
        if (price <= 0)
            return Result<DailyPrice>.Failure(new Error("DailyPrice.Price.Invalid", "Price must be greater than zero"));

        if (_dailyPrices.Any(p => p.ItemId == item.Id && p.Date == date))
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Exists", "Daily price already exists"));

        var dailyPrice = DailyPrice.Create(this, item, date, price, percentage, state);

        if (!dailyPrice.IsSuccess)
            return Result<DailyPrice>.Failure(dailyPrice.Error!);


        _dailyPrices.Add(dailyPrice.Data!);
        return Result<DailyPrice>.Success(dailyPrice.Data!);
    }

    private bool HasDailyPrice(Guid itemId, DateOnly date)
        => _dailyPrices.Any(p => p.ItemId == itemId && p.Date == date);

    private DailyPrice? GetDailyPrice(Guid itemId)
        => _dailyPrices.SingleOrDefault(p => p.ItemId == itemId);

    public void ClearDailyPrices()
        => _dailyPrices.Clear();
    #endregion

    #region Buy

    public Result<InventoryResult> BuyItem(Guid itemId, int quantity)
    {
        if (quantity <= 0)
            return Result<InventoryResult>.Failure(
                new Error("Inventory.Quantity.Invalid", "Quantity must be greater than zero."));

        if (quantity > PurchaseLimit)
            return Result<InventoryResult>.Failure(
                new Error("Player.PurchaseLimit.Insufficient", "Max purchase limit reached."));


        var dailyPrice = GetDailyPrice(itemId);
        if (dailyPrice is null)
            return Result<InventoryResult>.Failure(
                new Error("Player.DailyPrice.Missing", "Daily price not found."));

        var totalCost = dailyPrice.Price * quantity;

        if (!CanAfford(totalCost))
            return Result<InventoryResult>.Failure(
                new Error("Player.Balance.Insufficient", "Insufficient balance."));


        var result = AddToInventory(itemId, quantity);

        if (!result.IsSuccess)
            return Result<InventoryResult>.Failure(result.Error!);

        RaiseDomainEvent(
            new ItemPurchasedEvent(
                dailyPrice.Id,
                result.Data!.Inventory.PlayerId,
                quantity,
                totalCost));

        ConsumeLimit(quantity);
        DecreaseBalance(totalCost);

        return Result<InventoryResult>.Success(result.Data!);
    }
    #endregion

    #region Selling    

    public Result<Inventory> SellItem(Guid itemId, int quantity)
    {
        if (quantity <= 0)
            return Result<Inventory>.Failure(
                new Error("Inventory.Quantity.Invalid", "Quantity must be greater than zero."));

        var dailyPrice = GetDailyPrice(itemId);
        if (dailyPrice is null)
            return Result<Inventory>.Failure(
                new Error("Player.DailyPrice.Missing", "Daily price not found."));

        var revenue = dailyPrice.Price * quantity;

        var result = RemoveFromInventory(itemId, quantity);
        if (!result.IsSuccess)
            return Result<Inventory>.Failure(result.Error!);

        RaiseDomainEvent(
            new ItemSoldEvent(
                result.Data!.ItemId,
                result.Data!.PlayerId,
                quantity,
                revenue));

        IncreaseBalance(revenue);

        return result;
    }
    #endregion

    #region Inventory
    private Inventory? GetInventory(Guid itemId)
        => _inventoryItems.SingleOrDefault(i => i.ItemId == itemId);

    public record InventoryResult(Inventory Inventory, bool IsNew);
    public Result<InventoryResult> AddToInventory(Guid itemId, int quantity)
    {
        var existingItem = GetInventory(itemId);

        if (existingItem is null)
        {
            var createResult = Inventory.Create(Id, itemId, quantity);
            if (!createResult.IsSuccess)
                return Result<InventoryResult>.Failure(createResult.Error!);

            _inventoryItems.Add(createResult.Data!);

            return Result<InventoryResult>.Success(
                new InventoryResult(createResult.Data!, true));
        }

        existingItem.Increase(quantity);
        return Result<InventoryResult>.Success(
               new InventoryResult(existingItem, false));
    }
    public Result<Inventory> RemoveFromInventory(Guid itemId, int quantity)
    {
        var inventory = GetInventory(itemId);

        if (inventory is null)
            return Result<Inventory>.Failure(
                new Error("Inventory.NotFound", "Item not found in inventory."));

        if (inventory.IsQuantityExceeded(quantity))
            return Result<Inventory>.Failure(
                new Error("Inventory.Quantity.Exceeded", "The amount provided exceeded the total quantity of item"));

        if (!inventory.HasEnough(quantity))
            return Result<Inventory>.Failure(
                new Error("Inventory.Quantity.Insufficient", "Not enough quantity."));

        inventory.Decrease(quantity);
        if (inventory.Quantity == 0)
            _inventoryItems.Remove(inventory);

        return Result<Inventory>.Success(inventory);
    }
    #endregion
}