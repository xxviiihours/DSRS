using DSRS.Domain.Aggregates.Inventories;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.SellItem;

public record SellItemCommand(
    Guid PlayerId,
    Guid ItemId,
    int Quantity) : ICommand<Result<Inventory>>
{

}
