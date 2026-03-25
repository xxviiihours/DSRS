using DSRS.Application.Features.Players;

namespace DSRS.Application.Contracts;

public interface IPlayerQuery
{
    Task<PlayerDto> GetPlayerByIdAsync(Guid playerId);
    Task<PlayerDto> GetPlayerByName(string name);
    Task<List<PlayerDto>> GetPlayers(string query);
    Task<List<PlayerDto>> GetOtherPlayers(string query);
}
