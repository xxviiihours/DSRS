using DSRS.Application.Features.Authentications.Logout;
using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DSRS.Gateway.Endpoints.Authentications;

public class LogoutEndpoint(IMediator mediator) : EndpointWithoutRequest<IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("auth/logout");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Logout current user";
            // Document possible responses
            s.Responses[200] = "User logged out successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });
        // Add tags for API grouping
        Tags("Authentication");
        // Add additional metadata
        Description(builder => builder
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        await _mediator.Send(new LogoutCommand(), ct);
        return Results.Ok();
    }
}
