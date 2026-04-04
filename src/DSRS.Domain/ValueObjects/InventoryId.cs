using System;
using System.Collections.Generic;
using System.Text;
using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<Guid>(Conversions.SystemTextJson)]
public partial class InventoryId
{
    private static Validation Validate(Guid input)
    {
        bool isValid = true; // todo: your validation
        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
    }
    public static InventoryId New() => From(Guid.NewGuid());
    public static InventoryId Empty() => From(Guid.Empty);
    public bool IsEmpty() => Value == Guid.Empty;
    private static Guid NormalizeInput(Guid input)
    {
        // todo: normalize (sanitize) your input;
        return input;
    }
}
