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


        // maybe add domain event here

        return Result<Player>.Success(new Player(name, balance));
    }

    public Result<DailyPrice> AddDailyPrice(Item item, DateOnly date,
         decimal price, PriceState state)
    {
        if (price <= 0)
            return Result<DailyPrice>.Failure(new Error("DailyPrice.Price.Invalid", "Price must be greater than zero"));

        if (_dailyPrices.Any(p => p.ItemId == item.Id && p.Date == date))
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Exists", "Daily price already exists"));

        var dailyPrice = DailyPrice.Create(this, item, date, price, state);

        if (!dailyPrice.IsSuccess)
            return Result<DailyPrice>.Failure(dailyPrice.Error!);


        _dailyPrices.Add(dailyPrice.Data!);
        return Result<DailyPrice>.Success(dailyPrice.Data!);
    }

    public void ClearDailyPrices()
    {
        _dailyPrices.Clear();
    }
    public Result<Inventory> AddToInventory(Guid itemId, int quantity, decimal priceTotal, DistributionType type)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        var existingItem = _inventoryItems.SingleOrDefault(i => i.ItemId == itemId);
        if (existingItem is null)
        {
            var newItem = Inventory.Create(Id, itemId, quantity, priceTotal, type);

            if (!newItem.IsSuccess)
                return Result<Inventory>.Failure(newItem.Error!);

            _inventoryItems.Add(newItem.Data!);
            return Result<Inventory>.Success(newItem.Data!);
        }
        else
        {
            existingItem.AddQuantity(quantity);
        }

        return Result<Inventory>.Success(existingItem!);
    }

    // public void RemoveFromInventory(Guid itemId, int quantity)
    // {
    //     var existingItem = _inventoryItems.SingleOrDefault(i => i.ItemId == itemId);
    //     if (existingItem is null)
    //         throw new InvalidOperationException("Item not found in inventory.");

    //     existingItem.RemoveQuantity(quantity);
    // }

    public Result<Inventory> BuyItem(Guid itemId, int quantity)
    {
        if (quantity < 1)
            return Result<Inventory>.Failure(
                new Error("Inventory.Quantity.Empty", "Quantity must be greater than zero."));

        var price = _dailyPrices
            .Where(p => p.Item.Id == itemId)
            .Select(p => p.Price)
            .SingleOrDefault();

        if (price == 0)
            return Result<Inventory>.Failure(
                new Error("Player.DailyPrice.Empty", "Unable to initialize daily price."));

        var totalCost = price * quantity;

        if (Balance < totalCost)
            return Result<Inventory>.Failure(new Error("Player.Balance.Empty", "Insufficient balance."));

        Balance -= totalCost;

        return AddToInventory(itemId, quantity, totalCost, DistributionType.BUY);
    }

}