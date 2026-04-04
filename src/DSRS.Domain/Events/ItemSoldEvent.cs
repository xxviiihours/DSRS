using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using System;

namespace DSRS.Domain.Events;

public sealed class ItemSoldEvent(
    PlayerId playerId,
    DailyPriceId dailyPriceId,
    string itemName,
    Money totalCost,
    Money balance) : DomainEvent
{
    public PlayerId PlayerId { get; init; } = playerId;
    public DailyPriceId DailyPriceId { get; init; } = dailyPriceId;
    public string ItemName { get; set; } = itemName;
    public Money TotalCost { get; init; } = totalCost;
    public Money Balance { get; set; } = balance;
}
