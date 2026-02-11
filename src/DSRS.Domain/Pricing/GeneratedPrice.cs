using DSRS.SharedKernel.Enums;
using System;

namespace DSRS.Domain.Pricing;

public sealed record GeneratedPrice(decimal Price, PriceState State)
{
  public decimal Price { get; } = Price;
  public PriceState State { get; } = State;
}
