using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Dashboard.Queries;

public record GetDailyPricesPerItemCommand(Guid ItemId, Guid PlayerID) : ICommand<Result<List<DashboardDto>>> { }