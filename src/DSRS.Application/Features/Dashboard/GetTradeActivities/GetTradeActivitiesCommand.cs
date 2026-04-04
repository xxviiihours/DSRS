using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Dashboard.GetTradeActivities;

public record GetTradeActivitiesCommand(PlayerId PlayerId) : ICommand<Result<List<TradeActivityDto>>>
{
}
