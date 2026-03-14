using DSRS.Application.Features.Players;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface IIdentityService
{
    Task RegisterAccount(Player player, string email, string password);
    Task<PlayerDto> Authenticate(string username, string password);
    Task<Guid> FindByIdAsync(string username);
}
