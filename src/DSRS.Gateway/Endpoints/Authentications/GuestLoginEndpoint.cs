using DSRS.Application.Features.Authentications;
using DSRS.Application.Features.Authentications.GuestLogin;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Authentications;

public class GuestLoginEndpoint(IMediator mediator) : EndpointWithoutRequest<IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("auth/guest");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Login using Guest Account";
            // Document possible responses
            s.Responses[200] = "Guest logon successfully";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Authentication");

        // Add additional metadata
        Description(builder => builder
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GuestLoginCommand(), ct);

        return result.ToHttpResult(
            mapResponse => new AuthenticateResponse(mapResponse),
            locationBuilder => "",
            successStatusCode: 200
        );
    }
}
