using DSRS.Domain.Items;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Items.Get;

public record GetItemsCommand() : ICommand<Result<List<Item>>>
{
}
