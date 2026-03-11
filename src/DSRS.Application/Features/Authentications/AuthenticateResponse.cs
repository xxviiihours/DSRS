using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications;

public record AuthenticateResponse(Guid Id, string UserName)
{
    public Guid Id { get; set; } = Id;
    public string UserName { get; set; } = UserName;
}
