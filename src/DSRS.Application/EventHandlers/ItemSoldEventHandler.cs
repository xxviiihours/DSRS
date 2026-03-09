using DSRS.Application.Contracts;
using DSRS.Application.Notifications;
using DSRS.Domain.Aggregates.Distributions;
using Mediator;
using Microsoft.Extensions.Logging;
using System;

namespace DSRS.Application.EventHandlers;

public class ItemSoldEventHandler(IDistributionHistory distributionHistoryRepository,
   ILogger<ItemSoldEventHandler> logger) : INotificationHandler<ItemSoldNotification>
{
  private readonly IDistributionHistory _distributionHistoryRepository = distributionHistoryRepository;
  private readonly ILogger<ItemSoldEventHandler> _logger = logger;
  public async ValueTask Handle(ItemSoldNotification notification, CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogInformation("Creating distribution record for player {PlayerId} purchasing item {ItemId}",
          notification.Event.PlayerId, notification.Event.ItemId);

      var record = DistributionRecord.Create(
          notification.Event.ItemId,
          notification.Event.PlayerId,
          notification.Event.TotalCost,
          DistributionType.SELL).Data!;

      await _distributionHistoryRepository.CreateAsync(record);
      _logger.LogInformation("Distribution record created for player {PlayerId}", notification.Event.PlayerId);
      await Task.CompletedTask;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to handle ItemPurchasedNotification");
    }
  }
}
