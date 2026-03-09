using DSRS.Domain.Aggregates.Friendships;
using DSRS.Domain.Aggregates.Players;
using System;

namespace DSRS.Application.Contracts;

public interface ISocialRepository
{
  Task SendRequest(Friendship friendship);
  Task AcceptRequest(Friendship friendship);
  Task RejectRequest(Friendship friendship);

  Task<List<Friendship>> GetPendingRequests(Guid playerId);
  Task<List<Player>> GetFriendList(Guid playerId);

}
