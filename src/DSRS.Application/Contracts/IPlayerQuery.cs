using DSRS.Application.Features.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface IPlayerQuery
{
    Task<PlayerDto> GetPlayerByIdAsync(Guid playerId);
    Task<PlayerDto> GetPlayerByName(string name);
}
