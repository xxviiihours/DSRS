using DSRS.Application.Contracts;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Dashboard.Queries;

public class GetDailyPricesPerItemHandler(IDashboardQuery dashboardQuery) :
    ICommandHandler<GetDailyPricesPerItemCommand, Result<List<DashboardDto>>>
{
    private readonly IDashboardQuery _dashboardQuery = dashboardQuery;

    public async ValueTask<Result<List<DashboardDto>>> Handle(GetDailyPricesPerItemCommand command, CancellationToken cancellationToken)
    {
        var result = await _dashboardQuery.GetDailyPricesPerItem(command.ItemId, command.PlayerID);
        if (result == null)
            return Result<List<DashboardDto>>.Failure(
                new Error("Dashboard.Empty", "No daily prices found for this item."));

        return Result<List<DashboardDto>>.Success(result!);
    }
}
