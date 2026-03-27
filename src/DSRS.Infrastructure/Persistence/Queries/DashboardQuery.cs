using DSRS.Application.Contracts;
using DSRS.Application.Features.Dashboard;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace DSRS.Infrastructure.Persistence.Queries;

public class DashboardQuery(AppDbContext context) : IDashboardQuery
{
    private readonly AppDbContext _context = context;

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
