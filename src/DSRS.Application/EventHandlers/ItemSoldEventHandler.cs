using DSRS.Application.Contracts;
using DSRS.Application.Notifications;
using DSRS.Domain.Aggregates.Distributions;
using DSRS.Domain.Aggregates.Players;
using Mediator;
using Microsoft.Extensions.Logging;
using System;

namespace DSRS.Application.EventHandlers;

public class ItemSoldEventHandler(
    IDistributionHistoryRepository distributionHistoryRepository,
    ILogger<ItemSoldEventHandler> logger,
    IPlayerSnapshotRepository playerSnapshotRepository) : INotificationHandler<ItemSoldNotification>
{
    private readonly IDistributionHistoryRepository _distributionHistoryRepository = distributionHistoryRepository;
    private readonly IPlayerSnapshotRepository _playerSnapshotRepository = playerSnapshotRepository;
    private readonly ILogger<ItemSoldEventHandler> _logger = logger;
    public async ValueTask Handle(ItemSoldNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating distribution record for player {PlayerId} selling item {ItemId}",
                notification.Event.PlayerId, notification.Event.DailyPriceId);

            var record = DistributionRecord.Create(
                notification.Event.PlayerId,
                notification.Event.DailyPriceId,
                notification.Event.ItemName,
                notification.Event.TotalCost,
                DistributionType.SELL).Data!;

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
            _logger.LogError(ex, "Failed to handle ItemSoldNotification");
        }
    }
}
