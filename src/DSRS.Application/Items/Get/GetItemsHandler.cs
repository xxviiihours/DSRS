using DSRS.Application.Interfaces;
using DSRS.Domain.Items;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Items.Get;

public class GetItemsHandler(IItemRepository itemRepository) : ICommandHandler<GetItemsCommand, Result<List<Item>>>
{
	private readonly IItemRepository _itemRepository = itemRepository;
	public async ValueTask<Result<List<Item>>> Handle(GetItemsCommand command, CancellationToken cancellationToken)
	{
		var result = await _itemRepository.GetAllAsync();
		
		if (result.Count < 0)
			return Result<List<Item>>.Failure(
				new Error("Get.Items.Empty", "No Items Found.")
			);

		return Result<List<Item>>.Success(result);
	}
}
