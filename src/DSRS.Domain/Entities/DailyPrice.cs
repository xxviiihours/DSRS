using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Entities;

public sealed class DailyPrice : EntityBase<Guid>
{
    public Guid PlayerId { get; }
    public Guid ItemId { get; }
    public DateOnly Date { get; }

    public decimal Price { get; }
    public PriceState State { get; }

    public Player Player { get; }
    public Item Item { get; }
    private DailyPrice(Guid playerId, Guid itemId, 
        DateOnly date, decimal price, PriceState state, 
        Player player, Item item)
    {
        PlayerId = playerId;
        ItemId = itemId;
        Date = date;
        Price = price;
        State = state;
        Player = player;
        Item = item;
    }
    public static Result<DailyPrice> Create(
        Player player, Item item, DateOnly date, decimal price, PriceState state)
    {
        if (player == null)
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Player.Null", "Player cannot be null"));
        if (item == null)
            return Result<DailyPrice>.Failure(
                new Error("DailyPrice.Item.Null", "Item cannot be null"));

        // domain event could be raised here, e.g., DailyPriceGenerated
        
        return Result<DailyPrice>.Success(
            new DailyPrice(player.Id, item.Id, date, price, state, player, item));
    }
}
