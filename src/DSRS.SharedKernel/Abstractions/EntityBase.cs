namespace DSRS.SharedKernel.Abstractions;

public abstract class EntityBase<TId>
{
    public TId Id { get; protected init; } = default!;
}
