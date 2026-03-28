using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Dashboard.GetBalancePerformance;

public class GetBalancePerformanceHandler(IDashboardQuery dashboardQuery) :
    ICommandHandler<GetBalancePerformanceCommand, Result<List<BalancePerformanceDto>>>
{
    private readonly IDashboardQuery _dashboardQuery = dashboardQuery;

    public async ValueTask<Result<List<BalancePerformanceDto>>> Handle(GetBalancePerformanceCommand command, CancellationToken cancellationToken)
    {
        var result = await _dashboardQuery.GetBalancePerformanceData(command.PlayerId);

        return Result<List<BalancePerformanceDto>>.Success(result);
    }
}
