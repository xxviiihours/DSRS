using DSRS.Application.Features.Authentications.GuestLogin;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
            s.Responses[500] = "Internal server error";
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

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.Data!.Id.ToString()),
        };

        var identity = new ClaimsIdentity(authClaims, IdentityConstants.ApplicationScheme);

        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(identity)
        );

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => "",
            successStatusCode: 200
        );
    }
}
