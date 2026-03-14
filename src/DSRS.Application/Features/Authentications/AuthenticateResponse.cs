using DSRS.Application.Features.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications;

public record AuthenticateResponse(PlayerDto Player, bool IsLoggedIn)
{
    public PlayerDto Player { get; set; } = Player;
    public bool IsLoggedIn { get; set; } = IsLoggedIn;
}
