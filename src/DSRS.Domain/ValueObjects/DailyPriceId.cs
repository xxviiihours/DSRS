using System;
using System.Collections.Generic;
using System.Text;
using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<Guid>(Conversions.SystemTextJson)]
public partial class DailyPriceId
{
    private static Validation Validate(Guid id)
    {
        return id != Guid.Empty ? Validation.Ok : Validation.Invalid("DailyPriceId cannot be empty");
    }
    public static DailyPriceId New() => From(Guid.NewGuid());
    public static DailyPriceId Empty() => From(Guid.Empty);
    public bool IsEmpty() => Value == Guid.Empty;
    private static Guid NormalizeInput(Guid input)
    {
        // todo: normalize (sanitize) your input;
        return input;
    }
}
