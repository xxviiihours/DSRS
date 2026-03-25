using DSRS.Application.Features.Players;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.GetMarketPrices;

public record GetMarketPricesCommand(Guid Id) : ICommand<Result<PlayerDto>>
{

}
