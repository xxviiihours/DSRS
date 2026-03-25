using DSRS.Application.Contracts;
using DSRS.Domain.Aggregates.Friendships;
using DSRS.SharedKernel.Primitives;
using Mediator;
using System;

namespace DSRS.Application.Features.Socials.SendRequest;

public class SendFriendRequestHandler(ISocialRepository socialRepository,
     IUnitOfWork unitOfWork) : ICommandHandler<SendFriendRequestCommand, Result<Friendship>>
{
    private readonly ISocialRepository _socialRepository = socialRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async ValueTask<Result<Friendship>> Handle(SendFriendRequestCommand command, CancellationToken cancellationToken)
    {
        var request = Friendship.CreateRequest(command.RequesterId, command.AddresseeId);
        if (!request.IsSuccess)
            return Result<Friendship>.Failure(request.Error!);

        await _socialRepository.SendRequest(request.Data!);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Friendship>.Success(request.Data!);
    }
}
