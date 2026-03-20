using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DSRS.Application.Features.Accounts.Register;

public record UpgradeAccountCommand(
    Guid Id, 
    string Name, 
    string Email, 
    string Password) : ICommand<Result<PlayerDto>>
{
}
