using DSRS.Domain.Aggregates.Players;

namespace DSRS.Application.Contracts;

public interface IPlayerSnapshotRepository
{
    Task SaveBalance(PlayerBalanceSnapshot snapshot);
}
