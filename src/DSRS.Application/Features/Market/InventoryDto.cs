using DSRS.Application.Features.Items;
using DSRS.Domain.Inventories;
using System;

namespace DSRS.Application.Features.Market;

public class InventoryDto
{
    public Guid Id { get; set; }
    public ItemDto Item { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal PriceTotal { get; set; }
    public DistributionType DistributionType { get; set; }
}
