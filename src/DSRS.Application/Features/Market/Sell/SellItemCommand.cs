using DSRS.Domain.Inventories;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.Sell;

public record SellItemCommand(
    Guid PlayerId,
    Guid ItemId,
    int Quantity) : ICommand<Result<Inventory>>
{

}
