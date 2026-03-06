using DSRS.Domain.Events;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Notifications;

public sealed class ItemPurchasedNotification : INotification
{
    public ItemPurchasedEvent Event { get; }
    public ItemPurchasedNotification(ItemPurchasedEvent @event)
    {
        Event = @event;
    }
}