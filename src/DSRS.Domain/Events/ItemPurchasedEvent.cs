using DSRS.SharedKernel.Abstractions;

namespace DSRS.Domain.Events;

public sealed class ItemPurchasedEvent(
    Guid dailyPriceId, 
    Guid playerId, 
    int quantity, 
    decimal totalCost) : DomainEvent
{
    public Guid ItemId { get; init; } = dailyPriceId;
    public Guid PlayerId { get; init; } = playerId;
    public int Quantity { get; init; } = quantity;
    public decimal TotalCost { get; init; } = totalCost;
}
