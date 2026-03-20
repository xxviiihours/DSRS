using DSRS.Application.Features.Accounts.Register;
using DSRS.Application.Features.Players;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;

namespace DSRS.Gateway.Endpoints.Accounts;

public class RegisterAccountEndpoint(IMediator mediator) : Endpoint<RegisterAccountRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post(RegisterAccountRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register account";
            // Document possible responses
            s.Responses[201] = "Account created successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Accounts");

        // Add additional metadata
        Description(builder => builder
          .Accepts<RegisterAccountRequest>("application/json")
          .Produces<RegisterAccountResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(RegisterAccountRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RegisterAccountCommand(
                request.Name,
                request.Email,
                request.Password), ct);

        return result.ToHttpResult(
            mapResponse => new RegisterAccountResponse
            {
                Id = mapResponse.Id,
                Player = mapResponse
            },
            locationBuilder => $"{RegisterAccountRequest.Route}/{result.Data?.Id}",
            successStatusCode: StatusCodes.Status201Created
        );
    }
}

public class RegisterAccountRequest
{
    public const string Route = "/accounts/register";
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterAccountValidator : Validator<RegisterAccountRequest>
{
    public RegisterAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
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

public class RegisterAccountResponse
{
    public Guid Id { get; set; }
    public PlayerDto Player { get; set; } = new();
}