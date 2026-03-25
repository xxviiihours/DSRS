using DSRS.Domain.Aggregates.Inventories;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.PurchaseItem;

public record PurchaseItemCommand(
    Guid PlayerId,
    Guid ItemId,
    int Quantity) : ICommand<Result<Inventory>>
{

}
