using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Aggregates.Pricing;

public sealed class DailyPrice : AggregateRoot<DailyPriceId>
{
    public DateOnly Date { get; }
    public Money Price { get; }
    public decimal Percentage { get; set; }
    public PriceState State { get; }
    public ItemId ItemId { get; }
    public Item Item { get; private set; } = null!;
    public PlayerId PlayerId { get; }

    //private DailyPrice() { }
    internal DailyPrice(
        PlayerId playerId,
        ItemId itemId,
        DateOnly date,
        Money price,
        decimal percentage,
        PriceState state)
    {
        Id = DailyPriceId.New();
        PlayerId = playerId;
        ItemId = itemId;
        Date = date;
        Price = price;
        Percentage = percentage;
        State = state;
    }
    public static Result<DailyPrice> Create(
        PlayerId playerId,
        ItemId itemId,
        DateOnly date,
        Money price,
        decimal percentage,
        PriceState state)
    {
        if (playerId.IsEmpty())
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Player.Empty", "Player cannot be empty"));
        if (itemId.IsEmpty())
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Item.Empty", "Item cannot be empty"));

        // domain event could be raised here, e.g., DailyPriceGenerated

        return Result<DailyPrice>.Success(
            new DailyPrice(
                playerId,
                itemId,
                date,
                price,
                percentage,
                state));
    }
}
