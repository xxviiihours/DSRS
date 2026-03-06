using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DSRS.SharedKernel.Abstractions;

public abstract class AggregateRoot<TId> where TId : notnull
{
    public TId Id { get; protected init; } = default!;

    private readonly List<DomainEvent> _domainEvents = [];

    protected AggregateRoot(){ }

    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(DomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
