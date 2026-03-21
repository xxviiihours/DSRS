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
            WITH CurrentRanks AS (
                SELECT
                    Id,
                    Name,
                    Balance,
                    ROW_NUMBER() OVER (ORDER BY Balance DESC) AS RankToday
                FROM Players
                WHERE IsGuest = 0
            ),
            YesterdayRanks AS (
                SELECT
                    PlayerId,
                    ROW_NUMBER() OVER (ORDER BY Balance DESC) AS RankYesterday
                FROM PlayerBalanceSnapshots
                WHERE SnapshotDate = DATE('now','-1 day')
            )
            SELECT
                c.Id,
                c.Name,
                c.Balance AS TotalBalance,
                c.RankToday AS Rank,
                ROUND(
                    ((y.RankYesterday - c.RankToday) * 100.0) / y.RankYesterday,
                    2
                ) AS RankChangePercent
            FROM CurrentRanks c
            LEFT JOIN YesterdayRanks y
                ON c.Id = y.PlayerId
            ORDER BY c.RankToday
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
            WITH CurrentRanks AS (
                SELECT
                    Id,
                    Name,
                    Balance,
                    ROW_NUMBER() OVER (ORDER BY Balance DESC) AS RankToday
                FROM Players
            ),
            YesterdayRanks AS (
                SELECT
                    PlayerId,
                    ROW_NUMBER() OVER (ORDER BY Balance DESC) AS RankYesterday
                FROM PlayerBalanceSnapshots
                WHERE DATE(SnapshotDate) = DATE('now','-1 day')
            )
            SELECT
                c.Id,
                c.Name,
                c.Balance AS TotalBalance,
                c.RankToday AS Rank,
                ROUND(
                    ((y.RankYesterday - c.RankToday) * 100.0) / y.RankYesterday,
                    2
                ) AS RankChangePercent
            FROM CurrentRanks c
            LEFT JOIN YesterdayRanks y
                ON c.Id = y.PlayerId
            WHERE c.Id = {0}
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
