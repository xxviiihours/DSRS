using DSRS.Application.Features.Players;
using DSRS.Domain.Players;

namespace DSRS.Application.Contracts;

public interface IPlayerRepository
{
    Task CreateAsync(Player player);
    Task<Player> GetById(Guid Id);
    Task<Player> GetByIdWithDailyPrices(Guid id);
    Task<Player> GetByIdWithInventories(Guid id);
    Task<bool> NameExistsAsync(string name);
}
