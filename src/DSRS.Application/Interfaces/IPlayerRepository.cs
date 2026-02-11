using DSRS.Domain.Players;

namespace DSRS.Application.Interfaces;

public interface IPlayerRepository
{
    Task<bool> NameExistsAsync(string name);
    Task CreateAsync(Player player);
}
