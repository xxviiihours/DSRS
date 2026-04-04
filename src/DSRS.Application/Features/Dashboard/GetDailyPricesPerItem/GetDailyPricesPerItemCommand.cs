using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Dashboard.GetDailyPricesPerItem;

public record GetDailyPricesPerItemCommand(ItemId ItemId, PlayerId PlayerId) : ICommand<Result<List<DashboardDto>>> { }