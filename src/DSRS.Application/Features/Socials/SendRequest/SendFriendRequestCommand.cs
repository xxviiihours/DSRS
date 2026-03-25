using DSRS.Domain.Aggregates.Friendships;
using DSRS.SharedKernel.Primitives;
using Mediator;

namespace DSRS.Application.Features.Socials.SendRequest;

public record SendFriendRequestCommand(Guid RequesterId, Guid AddresseeId) : ICommand<Result<Friendship>>
{

}
