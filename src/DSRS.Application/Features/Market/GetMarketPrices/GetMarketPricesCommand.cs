using DSRS.Application.Features.Players;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.GetMarketPrices;

public record GetMarketPricesCommand(PlayerId Id) : ICommand<Result<PlayerDto>>
{

}
