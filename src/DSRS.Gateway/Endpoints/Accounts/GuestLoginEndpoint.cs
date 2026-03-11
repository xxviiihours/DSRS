using DSRS.Application.Features.Accounts.GuestLogin;
using DSRS.Application.Features.Items.Create;
using DSRS.Gateway.Endpoints.Items;
using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Principal;

namespace DSRS.Gateway.Endpoints.Accounts;

public class GuestLoginEndpoint(IMediator mediator) : EndpointWithoutRequest
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GuestLoginCommand(), ct);

        if (result.IsSuccess)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, result.Data!.Id.ToString()),
            };

            var identity = new ClaimsIdentity(authClaims, IdentityConstants.ApplicationScheme);

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity)
            );

            await Send.NoContentAsync(ct);
        }
       
    }
}
