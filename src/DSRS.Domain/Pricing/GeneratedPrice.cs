using DSRS.SharedKernel.Enums;
using System;

namespace DSRS.Domain.Pricing;

public sealed record GeneratedPrice(decimal Price, decimal Percentage, PriceState State)
{
  public decimal Price { get; } = Price;
  public decimal Percentage { get; set; } = Percentage;
  public PriceState State { get; } = State;
}
