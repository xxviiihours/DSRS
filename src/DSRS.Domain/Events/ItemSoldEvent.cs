using DSRS.SharedKernel.Abstractions;
using System;

namespace DSRS.Domain.Events;

public sealed class ItemSoldEvent(
    Guid dailyPriceId,
    Guid playerId,
    string itemName,
    int quantity,
    decimal totalCost,
    decimal balance) : DomainEvent
{
    public Guid ItemId { get; init; } = dailyPriceId;
    public Guid PlayerId { get; init; } = playerId;
    public string ItemName { get; set; } = itemName;
    public int Quantity { get; init; } = quantity;
    public decimal TotalCost { get; init; } = totalCost;
    public decimal Balance { get; set; } = balance;
}
