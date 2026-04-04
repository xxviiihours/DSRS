using DSRS.Application.Contracts;
using DSRS.Application.Notifications;
using DSRS.Domain.Aggregates.Distributions;
using DSRS.Domain.Aggregates.Players;
using Mediator;
using Microsoft.Extensions.Logging;

namespace DSRS.Application.EventHandlers;

public class ItemPurchasedEventHandler(
    IDistributionHistoryRepository distributionHistoryRepository,
    ILogger<ItemPurchasedEventHandler> logger,
    IPlayerSnapshotRepository playerSnapshotRepository) : INotificationHandler<ItemPurchasedNotification>
{
    private readonly IDistributionHistoryRepository _distributionHistoryRepository = distributionHistoryRepository;
    private readonly IPlayerSnapshotRepository _playerSnapshotRepository = playerSnapshotRepository;
    private readonly ILogger<ItemPurchasedEventHandler> _logger = logger;

    public async ValueTask Handle(ItemPurchasedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating distribution record for player {PlayerId} purchasing item {ItemId}",
                notification.Event.PlayerId, notification.Event.dailyPriceId);

            var record = DistributionRecord.Create(
                notification.Event.PlayerId,
                notification.Event.dailyPriceId,
                notification.Event.ItemName,
                notification.Event.TotalCost,
                DistributionType.BUY).Data!;

            await _distributionHistoryRepository.CreateAsync(record);

            var balanceSnapshot = PlayerBalanceSnapshot.Create(
                notification.Event.PlayerId,
                notification.Event.Balance).Data!;

            await _playerSnapshotRepository.SaveBalance(balanceSnapshot);

            _logger.LogInformation("Distribution record created for player {PlayerId}", notification.Event.PlayerId);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle ItemPurchasedNotification");
        }
    }
}
