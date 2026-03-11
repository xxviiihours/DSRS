using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Accounts;

public class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
