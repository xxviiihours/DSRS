using System;
using System.Collections.Generic;
using System.Text;
using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<Guid>(Conversions.SystemTextJson)]
public partial class ItemId
{
    private static Validation Validate(Guid id)
    {
        return id != Guid.Empty ? Validation.Ok : Validation.Invalid("ItemId cannot be empty");
    }

    public static ItemId New() => From(Guid.NewGuid());
    public static ItemId Empty() => From(Guid.Empty);
    public bool IsEmpty() => Value == Guid.Empty;
    private static Guid NormalizeInput(Guid input)
    {
        // todo: normalize (sanitize) your input;
        return input;
    }

}
