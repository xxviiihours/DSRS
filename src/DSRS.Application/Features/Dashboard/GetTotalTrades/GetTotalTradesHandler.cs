using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Dashboard.GetTotalTrades;

public class GetTotalTradesHandler(IDashboardQuery dashboardQuery) : 
    ICommandHandler<GetTotalTradesCommand, Result<List<TradeActivityDto>>>
{
    private readonly IDashboardQuery _dashboardQuery = dashboardQuery;

    public async ValueTask<Result<List<TradeActivityDto>>> Handle(
        GetTotalTradesCommand command, CancellationToken cancellationToken)
    {
        var result = await _dashboardQuery.GetTotalTrades(command.PlayerId);
        return Result<List<TradeActivityDto>>.Success(result);
    }
}
