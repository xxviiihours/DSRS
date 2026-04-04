using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;
using System;

namespace DSRS.Domain.Aggregates.Friendships;

public class Friendship : AggregateRoot<FriendshipId>
{
    private Friendship(PlayerId requesterId, PlayerId addresseeId, FriendshipPair pair)
    {
        Id = FriendshipId.New();
        RequesterId = requesterId;
        AddresseeId = addresseeId;
        Pair = pair;
        CreatedAt = DateTime.UtcNow;
        Status = FriendshipStatus.PENDING;

    }

    public PlayerId RequesterId { get; private set; }
    public PlayerId AddresseeId { get; private set; }
    public FriendshipPair Pair { get; private set; } = null!;
    public FriendshipStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime AcceptedAt { get; private set; }

    public Player Requester { get; private set; } = null!;
    public Player Addressee { get; private set; } = null!;

    public static Result<Friendship> CreateRequest(PlayerId requesterId, PlayerId addresseeId)
    {
        if (requesterId.IsEmpty())
            return Result<Friendship>.Failure(
              new Error("Friend.Request.Empty", "Requester Id cannot be empty."));

        if (addresseeId.IsEmpty())
            return Result<Friendship>.Failure(
              new Error("Friend.Addresee.Empty", "Addresee Id cannot be empty."));

        if (requesterId.Value == addresseeId.Value)
            return Result<Friendship>.Failure(
                new Error("Friendship.Request.SamePerson", "Cannot send friend request to yourself"));
        try
        {
            var pair = FriendshipPair.Create(requesterId, addresseeId);
            var friendship = new Friendship(requesterId, addresseeId, pair);

            return Result<Friendship>.Success(friendship);

        }
        catch (InvalidOperationException ex)
        {
            return Result<Friendship>.Failure(
              new Error("Friend.Request.Empty", ex.Message));
        }
    }

    public Result<Friendship> AcceptRequest()
    {
        if (Status != FriendshipStatus.PENDING)
            return Result<Friendship>.Failure(
                new Error("Friendship.Accept.NotPending", "Can only accept pending requests"));

        if (RequesterId.IsEmpty())
            return Result<Friendship>.Failure(
              new Error("Friend.Request.Empty", "Requester Id cannot be empty."));

        if(AddresseeId.IsEmpty())
            return Result<Friendship>.Failure(
              new Error("Friend.Addresee.Empty", "Addresee Id cannot be empty."));

        UpdateStatus(FriendshipStatus.ACCEPTED);
        AcceptedAt = DateTime.UtcNow;

        return Result<Friendship>.Success(this);
    }

    public Result<Friendship> RejectRequest(Friendship friendship)
    {
        if (RequesterId == Guid.Empty)
            return Result<Friendship>.Failure(
              new Error("Friend.Request.Empty", "Requester Id cannot be empty."));

        UpdateStatus(FriendshipStatus.REJECTED);
        return Result<Friendship>.Success(friendship);
    }

    private void UpdateStatus(FriendshipStatus status) => Status = status;
}
