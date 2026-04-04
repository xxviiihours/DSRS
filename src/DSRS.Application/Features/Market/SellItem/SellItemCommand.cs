using DSRS.Domain.Aggregates.Inventories;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.SellItem;

public record SellItemCommand(
    PlayerId PlayerId,
    ItemId ItemId,
    int Quantity) : ICommand<Result<Inventory>>
{

}
