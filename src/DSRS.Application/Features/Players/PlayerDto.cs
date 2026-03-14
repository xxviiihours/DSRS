using DSRS.Application.Features.DailyPrices;
using DSRS.Application.Features.Market;
using DSRS.SharedKernel.Mappings;
using System;

namespace DSRS.Application.Features.Players;

public record PlayerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int PurchaseLimit { get; set; }
    public bool IsGuest { get; set; }

    [MapFilter("Date", "Today")]
    public IReadOnlyList<DailyPriceDto> DailyPrices { get; set; } = [];
    public IReadOnlyList<InventoryDto> InventoryItems { get; set; } = [];
};
