using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Persistence.Repositories;

public class PlayerRepository(AppDbContext context,
     IDateTime dateTimeService) : IPlayerRepository
{
    private readonly AppDbContext _context = context;
    private readonly IDateTime _dateTimeService = dateTimeService;

    public async Task CreateAsync(Player player)
    {

        await _context.Players.AddAsync(player);
        await Task.CompletedTask;
    }

    public async Task<Player> FindGuestById(Guid id)
    {
        var result = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id && p.IsGuest);
        return result!;
    }

    public async Task<List<Player>> GetAllAsync()
    {
        var players = await _context.Players
            .Where(p => !p.IsGuest)
            .ToListAsync();

        return players;
    }

    public async Task<List<Player>> GetAllGuest()
    {
        var guestPlayers = await _context.Players
            .Where(p => p.IsGuest)
            .ToListAsync();

        return guestPlayers;
    }

    public async Task<Player> GetById(Guid Id)
    {
        var result = await _context.Players.FindAsync(Id);
        return result!;
    }

    public async Task<Player> GetByIdWithDailyPrices(Guid id)
    {
        var player = await _context.Players
            .Where(p => p.Id == id)
            .Include(p => p.InventoryItems)
                .ThenInclude(p => p.Item)
            .Include(p => p.DailyPrices
                    .Where(p => p.Date == _dateTimeService.DateToday))
                .ThenInclude(p => p.Item)
            .SingleOrDefaultAsync();

        return player!;
    }

    public async Task<Player> GetByIdWithInventories(Guid id)
    {
        var player = await _context.Players
            .Where(p => p.Id == id)
            .Include(p => p.DailyPrices
                .Where(p => p.Date == _dateTimeService.DateToday))
                .ThenInclude(p => p.Item)
            .Include(p => p.InventoryItems)
            .SingleOrDefaultAsync();

        return player!;
    }

    public async Task<Player> GetByName(string name)
    {
        var player = await _context.Players
            .Where(p => p.Name == name)
            .Include(p => p.InventoryItems)
                .ThenInclude(p => p.Item)
            .FirstOrDefaultAsync();

        return player!;
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Players
            .AsNoTracking()
            .AnyAsync(p => p.Name == name);
    }

    public async Task PatchAsync(Player player)
    {
        _context.Players.Update(player);
        await Task.CompletedTask;
    }
}
