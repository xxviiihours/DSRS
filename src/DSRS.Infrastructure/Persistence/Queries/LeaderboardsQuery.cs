using DSRS.Application.Contracts;
using DSRS.Application.Features.Leaderboards;
using DSRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace DSRS.Infrastructure.Persistence.Queries;

public class LeaderboardsQuery(AppDbContext context) : ILeaderboardsQuery
{
    private readonly AppDbContext _context = context;

    public async Task<List<PlayerLeaderboardDto>> GetTop20Players(Guid playerId)
    {
        var top20Sql = @"
            SELECT Id,
                   Name,
                   Balance AS TotalBalance,
                   ROW_NUMBER() OVER (ORDER BY Balance DESC) AS Rank
            FROM Players
            ORDER BY Balance DESC
            LIMIT 20
        ";

        var top20 = await _context.PlayerLeaderboards
            .FromSqlRaw(top20Sql)
            .AsNoTracking()
            .ToListAsync();


        var existing = top20.FirstOrDefault(p => p.Id == playerId);
        if (existing != null)
            return top20;

        var currentUserSql = @"
            SELECT *
            FROM (
                SELECT Id,
                       Name,
                       Balance AS TotalBalance,
                       ROW_NUMBER() OVER (ORDER BY Balance DESC) AS Rank
                FROM Players
            )
            WHERE Id = {0}
        ";

        var currentPlayer = await _context.PlayerLeaderboards
            .FromSqlRaw(currentUserSql, playerId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (currentPlayer == null)
            return top20;

        top20.Add(currentPlayer);

        return [.. top20.OrderBy(p => p.Rank)];
    }
}
