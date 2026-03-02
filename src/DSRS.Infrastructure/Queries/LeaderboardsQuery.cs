using DSRS.Application.Contracts;
using DSRS.Application.Features.Leaderboards;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace DSRS.Infrastructure.Queries;

public class LeaderboardsQuery(AppDbContext context) : ILeaderboardsQuery
{
    private readonly AppDbContext _context = context;

    public async Task<List<PlayerLeaderboardDto>> GetTop20Players(Guid playerId)
    {
        var top20 = await _context.Players
        .AsNoTracking()
        .OrderByDescending(p => p.Balance)
        .Take(20)
        .Select(p => new PlayerLeaderboardDto
        {
            Id = p.Id,
            Name = p.Name,
            TotalBalance = p.Balance,
        })
        .ToListAsync();

        var currentUser = await _context.Players
            .Where(p => p.Id == playerId)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Balance
            })
            .FirstOrDefaultAsync();

        if (currentUser == null)
            return top20;

        var rank = await _context.Players
            .AsNoTracking()
            .CountAsync(p => p.Balance > currentUser.Balance) + 1;

        var userDto = new PlayerLeaderboardDto
        {
            Id = currentUser.Id,
            Name = currentUser.Name + "(you)",
            TotalBalance = currentUser.Balance,
            Rank = rank
        };

        if (!top20.Any(p => p.Id == playerId))
        {
            top20.Add(userDto);
        }

        return [.. top20.OrderBy(p => p.Rank)];
    }
}
