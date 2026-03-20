using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;

namespace DSRS.Application.Contracts;

public interface IPlayerRepository
{
    Task CreateAsync(Player player);
    Task PatchAsync(Player player);
    Task<Player> GetById(Guid Id);
    Task<Player> GetByName(string name);
    Task<Player> GetByIdWithDailyPrices(Guid id);
    Task<Player> GetByIdWithInventories(Guid id);
    Task<bool> NameExistsAsync(string name);
    Task<Player> FindGuestById(Guid id);
}
