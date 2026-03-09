using DSRS.Domain.Aggregates.Friendships;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Socials.Accept;

public record SendFriendRequestCommand(Guid RequesterId, Guid AddresseeId) : ICommand<Result<Friendship>>
{

}
