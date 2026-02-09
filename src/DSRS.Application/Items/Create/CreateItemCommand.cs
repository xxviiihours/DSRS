using DSRS.Domain.Entities;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Items.Create;

public record CreateItemCommand(string Name, decimal BasePrice, 
  decimal Volatility) : ICommand<Result<Item>>
{
}
