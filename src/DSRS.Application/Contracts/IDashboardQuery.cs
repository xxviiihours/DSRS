using DSRS.Application.Features.Dashboard;
using DSRS.Domain.ValueObjects;
using System;

namespace DSRS.Application.Contracts;

public interface IDashboardQuery
{

    Task<List<DashboardDto>> GetDailyPricesPerItem(ItemId ItemId, PlayerId PlayerId);
    Task<List<TradeActivityDto>> GetRecentTradeActivities(PlayerId PlayerId);
    Task<List<BalancePerformanceDto>> GetBalancePerformanceData(PlayerId PlayerId);
    Task<List<TradeActivityDto>> GetTotalTrades(PlayerId PlayerId);
}
