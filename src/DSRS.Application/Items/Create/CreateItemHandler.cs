using DSRS.Application.Interfaces;
using DSRS.Domain.Items;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Interfaces;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System.Data.Common;

namespace DSRS.Application.Items.Create;

public class CreateItemHandler(IItemRepository itemRepository,
    IUnitOfWork unitOfWork, IDateTime dateTimeService) : ICommandHandler<CreateItemCommand, Result<Item>>
{
  private readonly IItemRepository _itemRepository = itemRepository;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly IDateTime _dateTimeService = dateTimeService;

  public async ValueTask<Result<Item>> Handle(CreateItemCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var item = Item.Create(command.Name, command.Description, command.BasePrice, command.Volatility);

      if (!item.IsSuccess)
        return Result<Item>.Failure(item.Error!);

      var dailyPrice = MarketPricingService.Generate(item.Data!);

      await _itemRepository.CreateAsync(item.Data!);
      await _unitOfWork.CommitAsync(cancellationToken);

      return Result<Item>.Success(item.Data!);
    }
    catch (DbException ex)
    {
      return Result<Item>.Failure(new Error("Database.Error", $"{ex.Message}"));
    }
  }
}
