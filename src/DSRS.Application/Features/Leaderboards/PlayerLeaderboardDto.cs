using System;

namespace DSRS.Application.Features.Leaderboards;

public class PlayerLeaderboardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalBalance { get; set; }
    public int Rank { get; set; }
}
