using DSRS.Application.Features.Accounts.Register;
using DSRS.Gateway.Common.Extensions;
using DSRS.Gateway.Endpoints.Items;
using FastEndpoints;
using FluentValidation;
using Mediator;

namespace DSRS.Gateway.Endpoints.Accounts;

public class RegisterAccountEndpoint(IMediator mediator) : Endpoint<RegisterAccountCommand, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post(RegisterAccountRequest.Route);
        Policies("authenticated");

        Summary(s =>
        {
            s.Summary = "Register account";
            // Document possible responses
            s.Responses[201] = "Account created successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error";
        });

        // Add tags for API grouping
        Tags("Accounts");

        // Add additional metadata
        Description(builder => builder
          .Accepts<RegisterAccountRequest>("application/json")
          .Produces<CreateItemResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(RegisterAccountCommand request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{RegisterAccountRequest.Route}/{result.Data?.Id}",
            successStatusCode: StatusCodes.Status201Created
        );
    }
}

public class RegisterAccountRequest
{
    public const string Route = "/accounts/register";
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterAccountValidator : Validator<RegisterAccountRequest>
{
    public RegisterAccountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Equal(p => p.ConfirmPassword)
            .WithMessage("Password does not matched.")
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