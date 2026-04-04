using DSRS.Application.Contracts;
using DSRS.Domain.Services;
using DSRS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DSRS.Infrastructure.Persistence.Services;

public class MarketService(
    AppDbContext context,
    ILogger<MarketService> logger) : IMarketService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<MarketService> _logger = logger;
    public async Task<int> GenerateDailyPricesAsync(DateOnly today, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MarketService:GenerateDailyPrice Started...");

        var players = await _context.Players
            .Where(p => !p.IsGuest)
            .ToListAsync(cancellationToken);

        var items = await _context.Items
            .ToListAsync(cancellationToken);

        int generatedCount = 0;
        foreach (var player in players)
        {
            foreach (var item in items)
            {
                bool exists = await _context.DailyPrices
                    .AnyAsync(p => p.PlayerId == player.Id &&
                        p.ItemId == item.Id &&
                        p.Date == today, cancellationToken);

                if (exists) continue;

                var generatedPrice = MarketPricingService.Generate(item);

                player.AddDailyPrice(
                    item.Id, 
                    today,
                    Money.From(generatedPrice.Price), 
                    generatedPrice.Percentage, 
                    generatedPrice.State);

                generatedCount++;
            }
        }

        _logger.LogInformation("MarketService:GenerateDailyPrice completed.");
        return generatedCount;
    }

    public async Task<int> GeneratePurchaseLimitAsync(DateOnly today, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MarketService:GeneratePurchaseLimit Started...");

        var players = await _context.Players
            .ToListAsync(cancellationToken);

        int generatedCount = 0;
        foreach (var player in players)
        {
            PlayerPurchaseService.GenerateDailyPurchaseLimit(player, today);
            generatedCount++;
        }

        _logger.LogInformation("MarketService:GeneratePurchaseLimit completed.");
        return generatedCount;
    }
}
