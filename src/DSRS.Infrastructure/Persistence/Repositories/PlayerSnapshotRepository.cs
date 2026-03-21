using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Players;

namespace DSRS.Infrastructure.Persistence.Repositories;

public class PlayerSnapshotRepository(AppDbContext context) : IPlayerSnapshotRepository
{
    private readonly AppDbContext _context = context;

    public async Task SaveBalance(PlayerBalanceSnapshot snapshot)
    {
        await _context.PlayerBalanceSnapshots.AddAsync(snapshot);
        await Task.CompletedTask;
    }
}
