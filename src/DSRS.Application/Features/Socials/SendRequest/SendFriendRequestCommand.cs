using DSRS.Domain.Aggregates.Friendships;
using DSRS.Domain.ValueObjects;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Socials.SendRequest;

public record SendFriendRequestCommand(PlayerId RequesterId, PlayerId AddresseeId) : ICommand<Result<Friendship>>
{

}
