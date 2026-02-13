using DSRS.Application.Features.DailyPrices;
using DSRS.Domain.Pricing;

namespace DSRS.Application.Interfaces;

public interface IDailyPriceRepository
{
    Task<List<DailyPriceDto>> GetDailyPricesByPlayerId(Guid id);
    Task CreateAsync(DailyPrice dailyPrice);
    Task CreateAllAsync(List<DailyPrice> dailyPrices);
}
