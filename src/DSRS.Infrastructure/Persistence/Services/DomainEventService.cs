using DSRS.Application.Contracts;
using DSRS.Application.Notifications;
using DSRS.Domain.Events;
using DSRS.SharedKernel.Abstractions;
using Mediator;
using Microsoft.Extensions.Logging;

namespace DSRS.Infrastructure.Persistence.Services;

public class DomainEventService(IMediator mediator, ILogger<DomainEventService> logger) : IDomainEventService
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DomainEventService> _logger = logger;

    public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is null)
            return;

        _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        var notification = CreateNotification(domainEvent);
        if (notification == null)
            _logger.LogWarning("No handler registered for domain event - {event}", domainEvent.GetType().Name);

        await _mediator.Publish(notification!, cancellationToken);
    }

    private static INotification? CreateNotification(DomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ItemPurchasedEvent e => new ItemPurchasedNotification(e),
            ItemSoldEvent e => new ItemSoldNotification(e),
            _ => null
        };
    }
}
