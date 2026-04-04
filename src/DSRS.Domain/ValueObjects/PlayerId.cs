using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<Guid>(Conversions.SystemTextJson)]
public partial class PlayerId
{
    public static PlayerId New() => From(Guid.NewGuid());
    public static PlayerId Empty() => From(Guid.NewGuid());
    public bool IsEmpty() => Value == Guid.Empty;
    private static Validation Validate(Guid id)
    {
        return id != Guid.Empty ? Validation.Ok : Validation.Invalid("Player id cannot be empty");
    }
    private static Guid NormalizeInput(Guid input)
    {
        // todo: normalize (sanitize) your input;
        return input;
    }
}
