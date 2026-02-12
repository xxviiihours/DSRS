using DSRS.Domain.Players;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Players.Get;

public record GetMarketPriceCommand(Guid Id) : ICommand<Result<Player>>
{

}
