using DSRS.Application.Features.Players;
using DSRS.Domain.ValueObjects;

namespace DSRS.Application.Contracts;

public interface IPlayerQuery
{
    Task<PlayerDto> GetPlayerByIdAsync(PlayerId playerId);
    Task<PlayerDto> GetPlayerByName(string name);
    Task<List<PlayerDto>> GetPlayers(string query);
    Task<List<PlayerDto>> GetOtherPlayers(string query);
}
