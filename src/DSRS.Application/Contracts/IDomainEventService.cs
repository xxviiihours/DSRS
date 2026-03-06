using DSRS.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Contracts;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken);
}
