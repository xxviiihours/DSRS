using DSRS.Domain.Items;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Players;

public sealed class Player : EntityBase<Guid>
{
    public string Name { get; } = string.Empty;
    public decimal Balance { get; }

    private readonly List<DailyPrice> _dailyPrices = [];
    public IReadOnlyCollection<DailyPrice> DailyPrices => _dailyPrices.AsReadOnly();

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

}