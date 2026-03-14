using DSRS.Application.Features.Authentications.GuestLogin;
using DSRS.Application.Features.Authentications.UserLogin;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DSRS.Gateway.Endpoints.Authentications;

public class UserLoginEndpoint(IMediator mediator) : Endpoint<AuthenticateRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post(AuthenticateRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Login with credentials";
            // Document possible responses
            s.Responses[200] = "Login successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[401] = "Unauthorized - invalid credentials";
            s.Responses[500] = "Internal server error";
        });

        // Add tags for API grouping
        Tags("Authentication");

        // Add additional metadata
        Description(builder => builder
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(AuthenticateRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UserLoginCommand(request.UserName, request.Password), ct);

        if (!result.IsSuccess)
            await Send.UnauthorizedAsync(ct);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => "",
            successStatusCode: 200
        );
    }
}
public class AuthenticateRequest
{
    public const string Route = "/auth/login";
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class AuthenticateValidator : Validator<AuthenticateRequest>
{
    public AuthenticateValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character."); ;
    }
}