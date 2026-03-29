using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;

namespace DSRS.Domain.Events;

public sealed class ItemPurchasedEvent(
    Guid playerId, 
    Guid dailyPriceId,
    string itemName,
    Money totalCost, 
    Money balance) : DomainEvent
{
    public Guid ItemId { get; init; } = dailyPriceId;
    public Guid PlayerId { get; init; } = playerId;
    public string ItemName { get; set; } = itemName;
    public Money TotalCost { get; set; } = totalCost;
    public Money Balance { get; set; } = balance;
}
