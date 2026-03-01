using DSRS.Domain.Distributions;
using DSRS.Domain.Inventories;
using DSRS.Domain.Items;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Players;

public sealed class Player : EntityBase<Guid>
{
    public string Name { get; } = string.Empty;
    public decimal Balance { get; private set; }

    private readonly List<DailyPrice> _dailyPrices = [];
    public IReadOnlyCollection<DailyPrice> DailyPrices => _dailyPrices.AsReadOnly();
    private readonly List<Inventory> _inventoryItems = [];
    public IReadOnlyCollection<Inventory> InventoryItems => _inventoryItems.AsReadOnly();
    private readonly List<DistributionRecord> _distributionHistory = [];
    public IReadOnlyCollection<DistributionRecord> DistributionHistory => _distributionHistory.AsReadOnly();

    private Player(string name, decimal balance)
    {
        Name = name;
        Balance = balance;
    }
    public static Result<Player> Create(string name, decimal balance)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Player>.Failure(
                new Error("Player.Name.Empty", "Player name cannot be empty"));

        if (balance < 0)
            return Result<Player>.Failure(
                new Error("Player.Balance.Negative", "Balance cannot be negative"));

        // maybe add domain event here

        return Result<Player>.Success(new Player(name, balance));
    }

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
    private bool CanAfford(decimal amount) => Balance >= amount;
    private void DecreaseBalance(decimal amount) => Balance -= amount;

    public Result<InventoryResult> BuyItem(Guid itemId, int quantity)
    {
        if (quantity <= 0)
            return Result<InventoryResult>.Failure(
                new Error("Inventory.Quantity.Invalid", "Quantity must be greater than zero."));

        var dailyPrice = GetDailyPrice(itemId);
        if (dailyPrice is null)
            return Result<InventoryResult>.Failure(
                new Error("Player.DailyPrice.Missing", "Daily price not found."));

        var totalCost = dailyPrice.Price * quantity;

        if (!CanAfford(totalCost))
            return Result<InventoryResult>.Failure(
                new Error("Player.Balance.Insufficient", "Insufficient balance."));

        DecreaseBalance(totalCost);

        var result = AddToInventory(itemId, quantity);

        var record = DistributionRecord.Create(
            result.Inventory.ItemId,
            result.Inventory.PlayerId,
            totalCost, DistributionType.BUY);

        _distributionHistory.Add(record.Data!);

        return Result<InventoryResult>.Success(result);
    }
    #endregion

    #region Selling    
    private void IncreaseBalance(decimal amount) => Balance += amount;

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
        IncreaseBalance(revenue);

        var result = RemoveFromInventory(itemId, quantity);

        if (!result.IsSuccess)
            return Result<Inventory>.Failure(result.Error!);

        var record = DistributionRecord.Create(
            result.Data!.ItemId,
            result.Data!.PlayerId,
            revenue, DistributionType.SELL);

        _distributionHistory.Add(record.Data!);

        return result;
    }
    #endregion

    #region Inventory
    private Inventory? GetInventory(Guid itemId)
        => _inventoryItems.SingleOrDefault(i => i.ItemId == itemId);

    public record InventoryResult(Inventory Inventory, bool IsNew);
    public InventoryResult AddToInventory(Guid itemId, int quantity)
    {
        var existingItem = GetInventory(itemId);

        if (existingItem is null)
        {
            var createResult = Inventory.Create(Id, itemId, quantity);

            _inventoryItems.Add(createResult.Data!);

            return new InventoryResult(createResult.Data!, true);
        }

        existingItem.Increase(quantity);
        return new InventoryResult(existingItem, false);
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