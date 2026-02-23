using DSRS.Application.Features.Dashboard;
using System;

namespace DSRS.Application.Contracts;

public interface IDashboardQuery
{

    Task<List<DashboardDto>> GetDailyPricesPerItem(Guid ItemId, Guid PlayerId);
}
