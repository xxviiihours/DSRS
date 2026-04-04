using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;

namespace DSRS.Domain.Events;

public sealed class ItemPurchasedEvent(
    PlayerId playerId, 
    DailyPriceId dailyPriceId,
    string itemName,
    Money totalCost, 
    Money balance) : DomainEvent
{
    public PlayerId PlayerId { get; init; } = playerId;
    public DailyPriceId dailyPriceId { get; init; } = dailyPriceId;
    public string ItemName { get; set; } = itemName;
    public Money TotalCost { get; set; } = totalCost;
    public Money Balance { get; set; } = balance;
}
