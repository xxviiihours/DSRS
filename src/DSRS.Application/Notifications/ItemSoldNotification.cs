using DSRS.Domain.Events;
using Mediator;
using System;

namespace DSRS.Application.Notifications;

public class ItemSoldNotification(ItemSoldEvent @event) : INotification
{
  public ItemSoldEvent Event { get; } = @event;
}
