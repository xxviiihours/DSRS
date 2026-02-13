using System;

namespace DSRS.Application.Features.Items;

public record ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal Volatility { get; set; }
}