using DSRS.Application.Contracts;
using DSRS.Application.Features.Dashboard;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Persistence.Queries;

public class DashboardQuery(AppDbContext context, IDateTime dateTimeService) : IDashboardQuery
{
    private readonly AppDbContext _context = context;
    private readonly IDateTime _dateTimeService = dateTimeService;

    public async Task<List<BalancePerformanceDto>> GetBalancePerformanceData(PlayerId PlayerId)
    {
        var fromDate = _dateTimeService.UtcNow.Date.AddDays(-30);

        var performance = await _context.PlayerBalanceSnapshots
            .Where(x => x.PlayerId == PlayerId && x.SnapshotDate >= fromDate)
            .GroupBy(x => new
            {
                x.PlayerId,
                Day = x.SnapshotDate.Date
            })
            .Select(g => new
            {
                g.Key.Day,
                StartBalance = g
                    .OrderBy(x => x.SnapshotDate)
                    .Select(x => x.Balance)
                    .First(),

                EndBalance = g
                    .OrderByDescending(x => x.SnapshotDate)
                    .Select(x => x.Balance)
                    .First()
            })
            .Select(x => new BalancePerformanceDto
            {
                Day = x.Day,
                Balance = x.EndBalance.Value,
                Profit = x.EndBalance.Value - x.StartBalance.Value
            })
            .ToListAsync();

        return performance;
    }

    public async Task<List<DashboardDto>> GetDailyPricesPerItem(ItemId ItemId, PlayerId PlayerId)
    {
        var result = await _context.DailyPrices
            .Where(p => p.ItemId == ItemId && p.PlayerId == PlayerId)
            .OrderByDescending(p => p.Date)
            .Take(7)
            .Select(p => new DashboardDto
            {
                BasePrice = p.Item.BasePrice,
                PreviousPrice = p.Price.Value,
                Percentage = p.Percentage,
                State = p.State,
                Date = p.Date
            }).ToListAsync();

        return result;
    }

    public async Task<List<TradeActivityDto>> GetRecentTradeActivities(PlayerId PlayerId)
    {
        var result = await _context.DistributionRecords
            .Where(p => p.PlayerId == PlayerId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new TradeActivityDto
            {
                ItemName = p.ItemName,
                TransactionDate = p.CreatedAt,
                Type = p.Type,
                PriceTotal = p.PriceTotal.Value
            })
            .ToListAsync();

        return result;
    }

    public async Task<List<TradeActivityDto>> GetTotalTrades(PlayerId PlayerId)
    {
        var weeksAgo = _dateTimeService.UtcNow.Date.AddDays(-6);
        var result = await _context.DistributionRecords
            .Where(p => p.PlayerId == PlayerId
                && p.CreatedAt >= weeksAgo)
            .GroupBy(g => g.CreatedAt.Date)
            .Select(x => new TradeActivityDto
            {
                TotalTrades = x.Count(),
                TransactionDate = x.Key
            })
            .ToListAsync();

        return result;
    }
}
