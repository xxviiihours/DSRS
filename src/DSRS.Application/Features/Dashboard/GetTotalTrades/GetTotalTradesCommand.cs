using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Dashboard.GetTotalTrades;

public record GetTotalTradesCommand(PlayerId PlayerId) : ICommand<Result<List<TradeActivityDto>>>
{
}
