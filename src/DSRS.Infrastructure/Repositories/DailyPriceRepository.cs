using DSRS.Application.Features.DailyPrices;
using DSRS.Application.Interfaces;
using DSRS.Domain.Pricing;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Repositories;

public class DailyPriceRepository(AppDbContext context) : IDailyPriceRepository
{
    private readonly AppDbContext _context = context;

    public async Task CreateAllAsync(List<DailyPrice> dailyPrices)
    {
        await _context.AddRangeAsync(dailyPrices);
        await Task.CompletedTask;
    }

    public async Task CreateAsync(DailyPrice dailyPrice)
    {
        await _context.AddAsync(dailyPrice);
        await Task.CompletedTask;
    }

    public async Task<List<DailyPriceDto>> GetDailyPricesByPlayerId(Guid id)
    {
        throw new NotImplementedException();
        // return await _context.DailyPrices
        //     .Include(p => p.Player)
        //     .Select(p => new DailyPriceDto(p.Id, p.Player.Id, p.Price, p.State))
        //     .ToListAsync();
    }
}
