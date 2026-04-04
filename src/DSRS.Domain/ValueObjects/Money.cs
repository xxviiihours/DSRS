using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Primitives;
using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<decimal>(Conversions.SystemTextJson)]
public partial class Money
{
    public bool CanAfford(decimal amount) => Value >= amount;
    public bool IsZero() => Value == 0;
    public bool IsNegative() => Value < 0;

    private static Validation Validate(decimal amount)
    {
        if(amount < 0)
        {
            return Validation.Invalid("Amount cannot be negative.");
        }

        if (amount > decimal.MaxValue / 2)
            return Validation.Invalid("Money exceeds maximum allowed value");

        return Validation.Ok;
    }
    private static decimal NormalizeInput(decimal input)
    {
        return input;
    }
}
