using DSRS.Application.Contracts;
using DSRS.Application.Notifications;
using DSRS.Domain.Aggregates.Distributions;
using DSRS.Domain.Events;
using Mediator;
using Microsoft.Extensions.Logging;

namespace DSRS.Application.Events;

public class ItemPurchasedEventHandler(IDistributionHistory distributionHistoryRepository,
    IUnitOfWork unitOfWork, ILogger<ItemPurchasedEventHandler> logger) : INotificationHandler<ItemPurchasedNotification>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDistributionHistory _distributionHistoryRepository = distributionHistoryRepository;
    private readonly ILogger<ItemPurchasedEventHandler> _logger = logger;

    public async ValueTask Handle(ItemPurchasedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating distribution record for player {PlayerId} purchasing item {ItemId}",
                notification.Event.PlayerId, notification.Event.ItemId);

            var record = DistributionRecord.Create(
                notification.Event.ItemId,
                notification.Event.PlayerId,
                notification.Event.TotalCost,
                DistributionType.BUY).Data!;

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
