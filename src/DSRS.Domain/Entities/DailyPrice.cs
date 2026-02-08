using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;

namespace DSRS.Domain.Entities;

public class DailyPrice(Player player, Item item, 
    DateOnly date, decimal price, PriceState state) : EntityBase<Guid>
{
    public Guid PlayerId { get; set; } = player.Id;
    public Guid ItemId { get; set; } = item.Id;
    public DateOnly Date { get; set; } = date;

    public decimal Price { get; set; } = price;
    public PriceState State { get; set; } = state;

    public Player Player { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
