using DSRS.Application.Contracts;
using DSRS.Application.Features.Dashboard;
using DSRS.Domain.Aggregates.Players;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace DSRS.Infrastructure.Persistence.Queries;

public class DashboardQuery(AppDbContext context) : IDashboardQuery
{
    private readonly AppDbContext _context = context;

    public async Task<List<BalancePerformanceDto>> GetBalancePerformanceData(Guid PlayerId)
    {
        var fromDate = DateTime.UtcNow.Date.AddDays(-30);

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
                Balance = x.EndBalance,
                Profit = x.EndBalance - x.StartBalance
            })
            .ToListAsync();

        return performance;
    }

    public async Task<List<DashboardDto>> GetDailyPricesPerItem(Guid ItemId, Guid PlayerId)
    {
        var result = await _context.DailyPrices
            .Where(p => p.ItemId == ItemId && p.PlayerId == PlayerId)
            .OrderByDescending(p => p.Date)
            .Take(7)
            .Select(p => new DashboardDto
            {
                BasePrice = p.Item.BasePrice,
                PreviousPrice = p.Price,
                Percentage = p.Percentage,
                State = p.State,
                Date = p.Date
            }).ToListAsync();

        return result;
    }

    public async Task<List<TradeActivityDto>> GetRecentTradeActivities(Guid PlayerId)
    {
        var result = await _context.DistributionRecords
            .Where(p => p.PlayerId == PlayerId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new TradeActivityDto
            {
                ItemName = p.ItemName,
                TransactionDate = p.CreatedAt,
                Type = p.Type,
                PriceTotal = p.PriceTotal
            })
            .ToListAsync();

        return result;
    }
}
