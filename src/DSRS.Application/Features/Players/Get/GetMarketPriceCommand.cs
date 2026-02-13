using DSRS.Domain.Players;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Players.Get;

public record GetMarketPriceCommand(Guid Id) : ICommand<Result<PlayerDto>>
{

}
