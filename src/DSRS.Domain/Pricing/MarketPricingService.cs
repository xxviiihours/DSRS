using DSRS.Domain.Items;
using DSRS.SharedKernel.Enums;

namespace DSRS.Domain.Pricing;

public class MarketPricingService
{
    public static GeneratedPrice Generate(Item item)
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

        return new GeneratedPrice(price, high ? PriceState.HIGH : PriceState.LOW);

    }

}
