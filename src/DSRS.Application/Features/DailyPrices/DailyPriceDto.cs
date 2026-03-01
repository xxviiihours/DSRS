using DSRS.Application.Features.Items;
using DSRS.SharedKernel.Enums;

namespace DSRS.Application.Features.DailyPrices;

public record DailyPriceDto
{
    public Guid Id { get; set; }
    public ItemDto? Item { get; set; }
    public decimal Price { get; set; }
    public decimal Percentage { get; set; }
    public DateOnly Date { get; set; }
    public PriceState State { get; set; }
}
