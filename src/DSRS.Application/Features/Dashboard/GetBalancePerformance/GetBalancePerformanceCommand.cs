using DSRS.SharedKernel.Primitives;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Features.Dashboard.GetBalancePerformance;

public record GetBalancePerformanceCommand(Guid PlayerId) : ICommand<Result<List<BalancePerformanceDto>>>
{
}
