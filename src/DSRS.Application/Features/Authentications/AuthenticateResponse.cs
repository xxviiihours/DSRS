using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Authentications;

public class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}
