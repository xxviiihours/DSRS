using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Dashboard.GetTotalTrades;

public record GetTotalTradesCommand(Guid PlayerId) : ICommand<Result<List<TradeActivityDto>>>
{
}
