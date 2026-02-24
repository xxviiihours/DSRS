using DSRS.Application.Contracts;
using DSRS.Application.Features.Dashboard;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace DSRS.Infrastructure.Queries;

public class DashboardQuery(AppDbContext context) : IDashboardQuery
{
    private readonly AppDbContext _context = context;

    public async Task<List<DashboardDto>> GetDailyPricesPerItem(Guid ItemId, Guid PlayerId)
    {
        var result = await _context.DailyPrices
            .Where(p => p.ItemId == ItemId && p.PlayerId == PlayerId)
            .Take(7)
            .Select(p => new DashboardDto
            {
                BasePrice = p.Item.BasePrice,
                PreviousPrice = p.Price,
                State = p.State,
                Date = p.Date
            }).ToListAsync();

        return result;
    }
}
