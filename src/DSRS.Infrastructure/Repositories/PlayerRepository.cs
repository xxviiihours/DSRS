using DSRS.Application.Interfaces;
using DSRS.Domain.Entities;
using DSRS.Infrastructure.Persistence;
using DSRS.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace DSRS.Infrastructure.Repositories;

public class PlayerRepository(AppDbContext context) : IPlayerRepository
{
    private AppDbContext _context = context;

    public async Task CreateAsync(Player player)
    {

        _context.Players.Add(player);
        await Task.CompletedTask;
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Players
            .AnyAsync(p => p.Name == name);
    }
}
