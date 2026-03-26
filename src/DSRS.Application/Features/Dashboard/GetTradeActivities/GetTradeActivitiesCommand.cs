using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Dashboard.GetTradeActivities;

public record GetTradeActivitiesCommand(Guid playerId) : ICommand<Result<List<TradeActivityDto>>>
{
}
