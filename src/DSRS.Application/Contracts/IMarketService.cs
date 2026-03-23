namespace DSRS.Application.Contracts;

public interface IMarketService
{
    Task<int> GenerateDailyPricesAsync(DateOnly today, CancellationToken cancellationToken = default);
    Task<int> GeneratePurchaseLimitAsync(DateOnly today, CancellationToken cancellationToken = default);
}
