using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Dashboard.GetBalancePerformance;

public record GetBalancePerformanceCommand(PlayerId PlayerId) : ICommand<Result<List<BalancePerformanceDto>>>
{
}
