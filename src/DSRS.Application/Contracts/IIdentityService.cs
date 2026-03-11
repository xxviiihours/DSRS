using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface IIdentityService
{
    Task RegisterAccount(Player player, string password);
    Task<string> Authenticate(string username, string password);
}
