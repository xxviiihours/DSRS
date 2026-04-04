using System;
using System.Collections.Generic;
using System.Text;
using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<Guid>(Conversions.SystemTextJson)]
public partial class FriendshipId
{
    private static Validation Validate(Guid id)
    {
        return id != Guid.Empty ? Validation.Ok 
            : Validation.Invalid("Friendship Id cannot be empty.");
    }
    public static FriendshipId New() => From(Guid.NewGuid());
    public bool IsEmpty() => Value == Guid.Empty;
    public override string ToString() => Value.ToString();
    private static Guid NormalizeInput(Guid input)
    {
        // todo: normalize (sanitize) your input;
        return input;
    }
}
