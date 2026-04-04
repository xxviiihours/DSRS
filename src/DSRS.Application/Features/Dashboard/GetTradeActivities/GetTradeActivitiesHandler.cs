using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Dashboard.GetTradeActivities;

public class GetTradeActivitiesHandler(IDashboardQuery dashboardQuery) :
    ICommandHandler<GetTradeActivitiesCommand, Result<List<TradeActivityDto>>>
{
    private readonly IDashboardQuery _dashboardQuery = dashboardQuery;

    public async ValueTask<Result<List<TradeActivityDto>>> Handle(
        GetTradeActivitiesCommand command, CancellationToken cancellationToken)
    {
        var result = await _dashboardQuery.GetRecentTradeActivities(command.PlayerId);
        
        return Result<List<TradeActivityDto>>.Success(result);
    }
}
