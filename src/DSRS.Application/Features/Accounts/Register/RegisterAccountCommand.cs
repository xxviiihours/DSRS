using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Accounts.Register;

public record RegisterAccountCommand(
    string Username, 
    string Email, 
    string Password) : ICommand<Result<PlayerDto>>
{
}
