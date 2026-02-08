using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;

namespace DSRS.Domain.Entities;

public class DailyPrice : EntityBase<Guid>
{
    public Guid PlayerId { get; private set; }
    public Guid ItemId { get; private set; }
    public DateOnly Date { get; private set; }

    public decimal Price { get; private set; }
    public PriceState State { get; private set; }

    public Player Player { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

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
        return Result<DailyPrice>.Success(new DailyPrice
            {
                Player = player,
                Item = item,
                Date = date,
                Price = price,
                State = state
            });
    }
}
