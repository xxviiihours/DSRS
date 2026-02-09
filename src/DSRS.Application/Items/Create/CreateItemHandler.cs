using DSRS.Application.Interfaces;
using DSRS.Domain.Entities;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System.Data.Common;

namespace DSRS.Application.Items.Create;

public class CreateItemHandler(IItemRepository itemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateItemCommand, Result<Item>>
{
  private readonly IItemRepository _itemRepository = itemRepository;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  public async ValueTask<Result<Item>> Handle(CreateItemCommand command, CancellationToken cancellationToken)
  {
    try
    {
      var item = Item.Create(command.Name, command.BasePrice, command.Volatility);

      if (!item.IsSuccess)
        return Result<Item>.Failure(item.Error!);

      await _itemRepository.Create(item.Data!);
      await _unitOfWork.CommitAsync(cancellationToken = default);

      return Result<Item>.Success(item.Data!);
    }
    catch (DbException ex)
    {
      return Result<Item>.Failure(new Error("Database.Error", $"{ex.Message}"));
    }
  }
}
