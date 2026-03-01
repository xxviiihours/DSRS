using DSRS.Application.Contracts;
using DSRS.Application.Features.Players;
using DSRS.Domain.Players;
using DSRS.Domain.Pricing;
using DSRS.SharedKernel.Interfaces;
using DSRS.SharedKernel.Mappings;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Market.Get;

public class GetMarketPriceHandler(IPlayerRepository playerRepository, IItemRepository itemRepository,
    IDateTime dateTimeService, IUnitOfWork unitOfWork) : ICommandHandler<GetMarketPriceCommand, Result<PlayerDto>>
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDateTime _dateTimeService = dateTimeService;
    public async ValueTask<Result<PlayerDto>> Handle(GetMarketPriceCommand command, CancellationToken cancellationToken)
    {
        var playerResult = await _playerRepository.GetByIdWithDailyPrices(command.Id);

        if (playerResult == null)
            return Result<PlayerDto>.Failure(new Error("Player.NotFound", "Player not found"));

        if (!playerResult.DailyPrices.Any(p => p.Date == _dateTimeService.DateToday))
        {
            var items = await _itemRepository.GetAllAsync();
            foreach (var item in items)
            {
                var generatedPrice = MarketPricingService.Generate(item);

                playerResult.AddDailyPrice(item, _dateTimeService.DateToday,
                    generatedPrice.Price, generatedPrice.Percentage, generatedPrice.State);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        var player = GenericMapper.Map<Player, PlayerDto>(playerResult);
        return Result<PlayerDto>.Success(player);
    }
}
