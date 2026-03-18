using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;

namespace DSRS.Application.Contracts;

public interface IIdentityService
{
    Task RegisterAccount(Player player, string email, string password);
    Task<PlayerDto> Authenticate(string username, string password);
    Task<Player> AuthenticateAsGuest(Player player);
    Task<Guid> FindByIdAsync(string username);

    Task SignOutAsync();
}
