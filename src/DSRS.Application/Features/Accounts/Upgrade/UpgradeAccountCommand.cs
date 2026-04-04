using DSRS.Application.Features.Players;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Accounts.Upgrade;

public record UpgradeAccountCommand(
    PlayerId Id, 
    string Name, 
    string Email, 
    string Password) : ICommand<Result<PlayerDto>>
{
}
