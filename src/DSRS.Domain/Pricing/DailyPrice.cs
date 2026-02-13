using DSRS.Domain.Items;
using DSRS.Domain.Players;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Pricing;

public sealed class DailyPrice : EntityBase<Guid>
{
    public DateOnly Date { get; }
    public decimal Price { get; }
    public PriceState State { get; }
    public Guid ItemId { get; }
    public Item Item { get; private set; } = null!;
    public Guid PlayerId { get; }
    // public Player Player { get; private set; } = null!;

    internal DailyPrice(Guid playerId,
        // Player player, 
        Item item,
        DateOnly date, 
        decimal price, 
        PriceState state)
    {
        PlayerId = playerId;
        ItemId = item.Id;
        Date = date;
        Price = price;
        State = state;
        // Player = player ?? throw new ArgumentNullException(nameof(player));
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }
    private DailyPrice() { }
    public static Result<DailyPrice> Create(
        Player player, 
        Item item, 
        DateOnly date, 
        decimal price, 
        PriceState state)
    {
        if (player == null)
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Player.Null", "Player cannot be null"));
        if (item == null)
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Item.Null", "Item cannot be null"));

        // domain event could be raised here, e.g., DailyPriceGenerated

        return Result<DailyPrice>.Success(
            new DailyPrice(
                player.Id, 
                item, 
                date, 
                price, 
                state));
    }
}
