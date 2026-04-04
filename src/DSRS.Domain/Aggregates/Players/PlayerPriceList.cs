using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.ValueObjects;
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
        ItemId itemId, 
        DateOnly date,
        Money price, 
        decimal percentage, 
        PriceState state)
    {
        if (price.IsZero() || price.IsNegative())
            return Result<DailyPrice>.Failure(new Error("DailyPrice.Price.Invalid", "Price must be greater than zero"));

        if (_dailyPrices.Any(p => p.ItemId == itemId && p.Date == date))
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Exists", "Daily price already exists"));

        var dailyPrice = DailyPrice.Create(null!, itemId, date, price, percentage, state);

        if (!dailyPrice.IsSuccess)
            return Result<DailyPrice>.Failure(dailyPrice.Error!);


        _dailyPrices.Add(dailyPrice.Data!);
        return Result<DailyPrice>.Success(dailyPrice.Data!);
    }

    public void ClearDailyPrices()
        => _dailyPrices.Clear();
    #endregion
}
