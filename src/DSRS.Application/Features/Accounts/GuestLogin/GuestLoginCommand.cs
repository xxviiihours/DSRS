using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Accounts.GuestLogin;

public record GuestLoginCommand : ICommand<Result<Player>>
{
}
