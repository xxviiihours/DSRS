using DSRS.Application.Features.DailyPrices;
using DSRS.SharedKernel.Mappings;
using System;

namespace DSRS.Application.Features.Players;

public record PlayerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }

    [MapFilter("Date", "Today")]
    public IReadOnlyList<DailyPriceDto> DailyPrices { get; set; } = [];
};
