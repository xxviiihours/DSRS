using DSRS.Domain.Players;

namespace DSRS.Application.Interfaces;

public interface IPlayerRepository
{
    Task CreateAsync(Player player);
    Task<Player> GetById(Guid Id);
    Task<Player> GetMarketPriceByPlayerId(Guid id);
    Task<bool> NameExistsAsync(string name);
}
