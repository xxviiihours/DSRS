using DSRS.Domain.Aggregates.Items;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Items.CreateItem;

public record CreateItemCommand(string Name,string Description,
   decimal BasePrice, decimal Volatility) : ICommand<Result<Item>>
{
}
