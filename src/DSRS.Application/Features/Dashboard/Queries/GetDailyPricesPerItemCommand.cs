using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Dashboard.Queries;

public record GetDailyPricesPerItemCommand(Guid ItemId, Guid PlayerId) : ICommand<Result<List<DashboardDto>>> { }