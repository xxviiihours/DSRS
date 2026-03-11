using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Aggregates.Players;

public class PlayerPriceList
{
    public Guid PlayerId { get; }

    private readonly List<DailyPrice> _dailyPrices = [];
    public IReadOnlyCollection<DailyPrice> DailyPrices => _dailyPrices.AsReadOnly();
    
    private PlayerPriceList(Guid playerId)
    {
        PlayerId = playerId;
    }

    public static PlayerPriceList Create(Guid playerId)
        => new(playerId);

    #region Daily Price
    public Result<DailyPrice> AddDailyPrice(
        Item item, 
        DateOnly date,
        decimal price, 
        decimal percentage, 
        PriceState state)
    {
        if (price <= 0)
            return Result<DailyPrice>.Failure(new Error("DailyPrice.Price.Invalid", "Price must be greater than zero"));

        if (_dailyPrices.Any(p => p.ItemId == item.Id && p.Date == date))
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Exists", "Daily price already exists"));

        var dailyPrice = DailyPrice.Create(null!, item, date, price, percentage, state);

        if (!dailyPrice.IsSuccess)
            return Result<DailyPrice>.Failure(dailyPrice.Error!);


        _dailyPrices.Add(dailyPrice.Data!);
        return Result<DailyPrice>.Success(dailyPrice.Data!);
    }

    public void ClearDailyPrices()
        => _dailyPrices.Clear();
    #endregion
}
