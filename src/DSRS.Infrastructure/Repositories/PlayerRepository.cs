using DSRS.Application.Interfaces;
using DSRS.Domain.Players;
using DSRS.Infrastructure.Persistence;
using DSRS.SharedKernel.Primitives;
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

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Players
            .AnyAsync(p => p.Name == name);
    }
}
