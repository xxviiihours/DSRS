using DSRS.Application.Features.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications;

public record AuthenticateResponse(PlayerDto Player)
{
    public PlayerDto Player { get; set; } = Player;
}
