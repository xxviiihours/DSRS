using DSRS.Domain.Aggregates.Items;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Items.GetItems;

public record GetItemsCommand() : ICommand<Result<List<Item>>>
{
}
