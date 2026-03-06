namespace DSRS.SharedKernel.Abstractions;

public abstract class EntityBase<TId> where TId : notnull
{
    public TId Id { get; protected init; } = default!;

    protected EntityBase() { }

    protected EntityBase(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj) =>
        obj is EntityBase<TId> entity && EqualityComparer<TId>.Default.Equals(Id, entity.Id);

    public override int GetHashCode() =>
        Id.GetHashCode();
}
