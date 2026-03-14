using DSRS.Application.Features.Authentications;
using DSRS.Application.Features.Authentications.GuestLogin;
using DSRS.Domain.Aggregates.Players;
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
            new(ClaimTypes.Name, result.Data!.Name),
            new("is_guest", result.Data!.IsGuest.ToString())
        };

        var identity = new ClaimsIdentity(authClaims, IdentityConstants.ApplicationScheme);

        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        return result.ToHttpResult(
            mapResponse => new AuthenticateResponse(mapResponse, identity.IsAuthenticated),
            locationBuilder => "",
            successStatusCode: 200
        );
    }
}
