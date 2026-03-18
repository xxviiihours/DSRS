using DSRS.Application.Features.Authentications;
using DSRS.Application.Features.Authentications.InitAuth;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Authentications;

public class InitPlayerAuthEndpoint(IMediator mediator) : EndpointWithoutRequest<IResult>
{
    private readonly IMediator _mediator = mediator;


    public override void Configure()
    {
        Get("auth/init");
        Policies("authenticated");
        Summary(s =>
        {
            s.Summary = "Initialize player authentication";
            // Document possible responses
            s.Responses[200] = "Player authentication initialized successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[500] = "Internal server error";
        });
        // Add tags for API grouping
        Tags("Authentication");
        // Add additional metadata
        Description(builder => builder
          .Produces<AuthenticateResponse>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var player = await _mediator.Send(new InitAuthCommand());

         return player.ToHttpResult(
             mapResponse => new AuthenticateResponse(player.Data!, true),
             locationBuilder => "",
             successStatusCode: StatusCodes.Status200OK);
    }
}