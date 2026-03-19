using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DSRS.Application.Features.Accounts.Register;

public record RegisterAccountCommand(
    string Username, 
    string Email, 
    string Password) : ICommand<Result<Player>>
{
}
