using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Abstractions;
using DSRS.SharedKernel.Enums;
using DSRS.SharedKernel.Primitives;
using System;

namespace DSRS.Domain.Aggregates.Friendships;

public class Friendship : AggregateRoot<Guid>
{
  private Friendship(Guid requesterId, Guid addresseeId)
  {
    var pair = new FriendshipPair(requesterId, addresseeId);
    RequesterId = requesterId;
    AddresseeId = addresseeId;
    CreatedAt = DateTime.UtcNow;

    PlayerA = pair.PlayerA;
    PlayerB = pair.PlayerB;

  }

  public Guid RequesterId { get; private set; }
  public Guid AddresseeId { get; private set; }
  public Guid PlayerA { get; private set; }
  public Guid PlayerB { get; private set; }
  public FriendshipStatus Status { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime AcceptedAt { get; private set; }

  public Player Requester { get; private set; } = null!;
  public Player Addressee { get; private set; } = null!;

  public static Result<Friendship> CreateRequest(Guid requesterId, Guid addresseeId)
  {
    if (requesterId == Guid.Empty)
      return Result<Friendship>.Failure(
        new Error("Friend.Request.Empty", "Requester Id cannot be empty."));

    if (addresseeId == Guid.Empty)
      return Result<Friendship>.Failure(
        new Error("Friend.Request.Empty", "Requester Id cannot be empty."));


    return Result<Friendship>.Success(new Friendship(requesterId, addresseeId));
  }

  public Result<Friendship> AcceptRequest(Friendship friendship)
  {
    if (friendship.RequesterId == Guid.Empty)
      return Result<Friendship>.Failure(
        new Error("Friend.Request.Empty", "Requester Id cannot be empty."));

    UpdateStatus(FriendshipStatus.ACCEPTED);

    return Result<Friendship>.Success(friendship);
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
