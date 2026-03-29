using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.SharedKernel.Abstractions;

public abstract class ValueObject<T> where T : ValueObject<T>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is not T other)
            return false;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(0, (current, next) =>
                HashCode.Combine(current, next));
    }

    public override string ToString()
    {
        var components = GetEqualityComponents()
            .Select(c => c?.ToString() ?? "null");
        return $"{GetType().Name}({string.Join(", ", components)})";
    }

    public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right)
        => !(left == right);
}
