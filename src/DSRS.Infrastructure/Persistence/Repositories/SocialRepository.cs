using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Friendships;
using DSRS.Domain.Aggregates.Players;
using DSRS.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace DSRS.Infrastructure.Persistence.Repositories;

public class SocialRepository(AppDbContext context) : ISocialRepository
{
  private readonly AppDbContext _context = context;

  public async Task AcceptRequest(Friendship friendship)
  {
    _context.Friendships.Update(friendship);

    await Task.CompletedTask;
  }

  public async Task<List<Player>> GetFriendList(Guid playerId)
  {
    var result = await _context.Friendships
        .Where(x =>
            (x.RequesterId == playerId || x.AddresseeId == playerId) &&
            x.Status == FriendshipStatus.ACCEPTED)
        .Select(x => x.RequesterId == playerId
            ? x.Addressee
            : x.Requester)
        .ToListAsync();

    return result;
  }

  public async Task<List<Friendship>> GetPendingRequests(Guid playerId)
  {
    var requests = await _context.Friendships
        .Where(x =>
            x.AddresseeId == playerId &&
            x.Status == FriendshipStatus.PENDING)
        .Include(x => x.Requester)
        .ToListAsync();

    return requests;
  }

  public async Task RejectRequest(Friendship friendship)
  {
    _context.Friendships.Update(friendship);
    await Task.CompletedTask;
  }

  public async Task SendRequest(Friendship friendship)
  {
    _context.Friendships.Add(friendship);
    await Task.CompletedTask;
  }
}
