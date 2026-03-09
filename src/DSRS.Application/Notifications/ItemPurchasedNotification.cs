using DSRS.Domain.Events;
using Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Application.Notifications;

public sealed class ItemPurchasedNotification(ItemPurchasedEvent @event) : INotification
{
    public ItemPurchasedEvent Event { get; } = @event;
}