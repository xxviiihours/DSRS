using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.SharedKernel.Abstractions;


public abstract class DomainEvent
{
    public Guid AggregateId { get; protected set; }

    public DateTimeOffset OccurredAt { get; protected set; } = DateTimeOffset.UtcNow;

    public int AggregateVersion { get; protected set; }

    public Guid EventId { get; } = Guid.NewGuid();
}