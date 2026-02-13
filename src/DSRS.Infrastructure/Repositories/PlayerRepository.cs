using DSRS.Application.Interfaces;
using DSRS.Domain.Players;
using DSRS.Infrastructure.Extensions;
using DSRS.Infrastructure.Persistence;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Repositories;

public class PlayerRepository(AppDbContext context) : IPlayerRepository
{
    private readonly AppDbContext _context = context;

    public async Task CreateAsync(Player player)
    {

        await _context.Players.AddAsync(player);
        await Task.CompletedTask;
    }

    public async Task<Player> GetById(Guid Id)
    {
        var result = await _context.Players.FindAsync(Id);
        return result!;
    }

    public async Task<Player> GetByIdWithDailyPrices(Guid id)
    {
        var player = await _context.Players
            .Include(p => p.DailyPrices)
                .ThenInclude(p => p.Item)
            .FirstOrDefaultAsync(p => p.Id == id);

        return player!;
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Players
            .AnyAsync(p => p.Name == name);
    }
}
