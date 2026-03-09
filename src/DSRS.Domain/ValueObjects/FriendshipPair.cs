using DSRS.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSRS.Domain.ValueObjects;

public sealed record FriendshipPair(Guid PlayerA, Guid PlayerB)
{
    public static FriendshipPair Create(Guid p1, Guid p2)
    {
        if (p1 == p2)
            throw new ArgumentException("Cannot friend yourself.");

        return p1.CompareTo(p2) < 0
            ? new FriendshipPair(p1, p2)
            : new FriendshipPair(p2, p1);
    }
}
