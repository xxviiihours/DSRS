using DSRS.Application.Contracts;
using DSRS.SharedKernel.Abstractions;
using Mediator;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace DSRS.Infrastructure.Persistence;

public class EFUnitOfWork(AppDbContext context, 
    IDomainEventService eventService,
    ILogger<EFUnitOfWork> logger) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private readonly IDomainEventService _eventService = eventService;
    private readonly ILogger<EFUnitOfWork> _logger = logger;
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {

            var events = CollectDomainEvents();

            await _context.SaveChangesAsync(cancellationToken);

            await PublishEventsAsync(events, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            // Clear events
            ClearDomainEvents();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task PublishEventsAsync(IEnumerable<DomainEvent> events,CancellationToken cancellationToken)
    {
        foreach (var @event in events)
        {
            try
            {
                _logger.LogDebug("Publishing domain event {EventType} (ID: {EventId})",@event.GetType().Name, @event.EventId);

                await _eventService.Publish(@event, cancellationToken);

                _logger.LogDebug("Domain event {EventType} published successfully", @event.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing domain event {EventType} (ID: {EventId})",@event.GetType().Name,@event.EventId);
                throw;
            }
        }
    }

    private List<DomainEvent> CollectDomainEvents()
    {
        var events = new List<DomainEvent>();
        var entries = _context.ChangeTracker.Entries<AggregateRoot<Guid>>();

        foreach (var entry in entries)
        {
            var aggregate = entry.Entity;
            events.AddRange(aggregate.DomainEvents);
        }

        return events;
    }

    private void ClearDomainEvents()
    {
        var entries = _context.ChangeTracker.Entries<AggregateRoot<Guid>>();
        foreach (var entry in entries)
        {
            entry.Entity.ClearDomainEvents();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
