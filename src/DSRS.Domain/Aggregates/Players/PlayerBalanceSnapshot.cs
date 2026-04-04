using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Aggregates.Players;

public class PlayerBalanceSnapshot : AggregateRoot<PlayerBalanceSnapshotId>
{
    public PlayerId PlayerId { get; private set; }
    public Money Balance { get; private set; }
    public DateTime SnapshotDate { get; private set; }

    //private PlayerBalanceSnapshot() { }
    internal PlayerBalanceSnapshot(PlayerId playerId, Money balance)
    {
        PlayerId = playerId;
        Balance = balance;
        SnapshotDate = DateTime.UtcNow;
    }

    public static Result<PlayerBalanceSnapshot> Create(PlayerId playerId, Money balance)
    {
        if(playerId == Guid.Empty)
            return Result<PlayerBalanceSnapshot>.Failure(
                new Error("Player.Id.Empty","PlayerId cannot be empty."));

        return Result<PlayerBalanceSnapshot>.Success(
            new PlayerBalanceSnapshot(playerId, balance));
    }
}
