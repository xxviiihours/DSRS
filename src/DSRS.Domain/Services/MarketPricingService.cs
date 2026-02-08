using DSRS.Domain.Entities;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Services;

public class MarketPricingService
{
    public static Result<DailyPrice> Generate(
             Player player,
             Item item,
             DateOnly date)
    {
        bool high = Random.Shared.NextDouble() > 0.5;

        var min = high
            ? item.BasePrice
            : item.BasePrice * (1 - item.Volatility);

        var max = high
            ? item.BasePrice * (1 + item.Volatility)
            : item.BasePrice;

        var price = Math.Round(
            min + (decimal)Random.Shared.NextDouble() * (max - min));

        var dailyPrice = DailyPrice.Create(player, item, date, price, high ? PriceState.HIGH : PriceState.LOW);
        if(!dailyPrice.IsSuccess)
        {
            return Result<DailyPrice>.Failure(dailyPrice.Error!);
        }

        return Result<DailyPrice>.Success(dailyPrice.Data!);
    }

}
