using DSRS.Application.Interfaces;
using DSRS.Domain.Players;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Interfaces;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Players.Get;

public class GetMarketPriceHandler(IPlayerRepository playerRepository, IItemRepository itemRepository,
    IDateTime dateTimeService) : ICommandHandler<GetMarketPriceCommand, Result<Player>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IDateTime _dateTimeService = dateTimeService;
    public async ValueTask<Result<Player>> Handle(GetMarketPriceCommand command, CancellationToken cancellationToken)
    {
        var result = await _playerRepository.GetMarketPriceByPlayerId(command.Id);

        if (result.DailyPrices.Count < 1)
        {
            var items = await _itemRepository.GetAllAsync();
            foreach (var item in items)
            {
                var generatedPrice = MarketPricingService.Generate(item);

                result.AddDailyPrice(item, _dateTimeService.DateToday,
                    generatedPrice.Price, generatedPrice.State);
            }
        }

        return Result<Player>.Success(result);
    }
}
