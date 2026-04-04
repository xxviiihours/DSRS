using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.ValueObjects;

namespace DSRS.Application.Contracts;

public interface IPlayerRepository
{
    Task CreateAsync(Player player);
    Task PatchAsync(Player player);
    Task<List<Player>> GetAllAsync();
    Task<List<Player>> GetAllGuest();
    Task<Player> GetById(PlayerId Id);
    Task<Player> GetByName(string name);
    Task<Player> GetByIdWithDailyPrices(PlayerId id);
    Task<Player> GetByIdWithInventories(PlayerId id);
    Task<bool> NameExistsAsync(string name);
    Task<Player> FindGuestById(PlayerId id);
}
