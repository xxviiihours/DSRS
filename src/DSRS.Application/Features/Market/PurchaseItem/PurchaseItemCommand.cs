using DSRS.Domain.Aggregates.Inventories;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.PurchaseItem;

public record PurchaseItemCommand(
    PlayerId PlayerId,
    ItemId ItemId,
    int Quantity) : ICommand<Result<Inventory>>
{

}
