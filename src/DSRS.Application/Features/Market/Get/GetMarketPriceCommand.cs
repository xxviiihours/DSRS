using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.Get;

public record GetMarketPriceCommand(Guid Id) : ICommand<Result<PlayerDto>>
{

}
