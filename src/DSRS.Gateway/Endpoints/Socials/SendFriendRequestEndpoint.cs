using DSRS.Application.Features.Socials.SendRequest;
using DSRS.Domain.ValueObjects;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;
using System;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Socials;

public class SendFriendRequestEndpoint(IMediator mediator) : Endpoint<SendFriendRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post(SendFriendRequest.Route);
        Policies("Authenticated", "RegistedUser");
        Summary(s =>
        {
            s.Summary = "Initiates Friend Requests";
            s.Description = "Sends friend requests using RequesterId and AddresseeId";
            // Document possible responses
            s.Responses[201] = "Friend request created successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "Unable to find Addresee using the parameters provided.";
            s.Responses[500] = "Internal server error occurred while processing the request";
        });
        // Add tags for API grouping
        Tags("Socials");
        // Add additional metadata
        Description(builder => builder
          .Accepts<SendFriendRequest>("application/json")
          .Produces<SendFriendResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(404)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(SendFriendRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SendFriendRequestCommand(
                PlayerId.From(Guid.Parse(req.RequesterId)), 
                PlayerId.From(Guid.Parse(req.AddresseeId))), ct);

        return result.ToHttpResult(
            mapResponse => new SendFriendResponse(result.Data!.Id.Value),
            locationBuilder => SendFriendRequest.Route,
            successStatusCode: StatusCodes.Status201Created
        );
    }
}

public record SendFriendResponse(Guid Id)
{
    public Guid Id { get; set; } = Id;

}

public class SendFriendRequest
{
    public const string Route = "/socials/send";
    [Required]
    public string RequesterId { get; set; } = string.Empty;
    [Required]
    public string AddresseeId { get; set; } = string.Empty;
}