using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Aggregates.Players;

public class PlayerBalanceSnapshot : AggregateRoot<Guid>
{
    public Guid PlayerId { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime SnapshotDate { get; private set; }

    internal PlayerBalanceSnapshot(Guid playerId, decimal balance)
    {
        PlayerId = playerId;
        Balance = balance;
        SnapshotDate = DateTime.UtcNow;
    }

    public static Result<PlayerBalanceSnapshot> Create(Guid playerId, decimal balance)
    {
        if(playerId == Guid.Empty)
            return Result<PlayerBalanceSnapshot>.Failure(
                new Error("Player.Id.Empty","PlayerId cannot be empty."));

        return Result<PlayerBalanceSnapshot>.Success(
            new PlayerBalanceSnapshot(playerId, balance));
    }
}
