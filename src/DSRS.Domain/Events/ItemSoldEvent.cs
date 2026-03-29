using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using System;

namespace DSRS.Domain.Events;

public sealed class ItemSoldEvent(
    Guid dailyPriceId,
    Guid playerId,
    string itemName,
    Money totalCost,
    Money balance) : DomainEvent
{
    public Guid ItemId { get; init; } = dailyPriceId;
    public Guid PlayerId { get; init; } = playerId;
    public string ItemName { get; set; } = itemName;
    public Money TotalCost { get; init; } = totalCost;
    public Money Balance { get; set; } = balance;
}
