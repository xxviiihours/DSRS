using DSRS.Application.Features.Accounts.Upgrade;
using DSRS.Application.Features.Players;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;

namespace DSRS.Gateway.Endpoints.Accounts;

public class UpgradeAccountEndpoint(IMediator mediator) : Endpoint<UpgradeAccountRequest, IResult>
{

    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Patch(UpgradeAccountRequest.Route);
        Policies("Authenticated");
        Summary(s =>
        {
            s.Summary = "Upgrade account";
            // Document possible responses
            s.Responses[201] = "Account upgraded successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Accounts");

        // Add additional metadata
        Description(builder => builder
          .Accepts<UpgradeAccountRequest>("application/json")
          .Produces(204)
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(UpgradeAccountRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpgradeAccountCommand(
                Guid.Parse(request.Id), 
                request.Name, 
                request.Email, 
                request.Password), ct);

        return result.ToHttpResult(
            mapresponse => new UpgradeAccountResponse
            {
                Id = mapresponse.Id,
                Player = mapresponse
            },
            locationBuilder => $"{RegisterAccountRequest.Route}/{result.Data?.Id}",
            successStatusCode: StatusCodes.Status200OK);
    }
}

public class UpgradeAccountRequest
{
    public const string Route = "/accounts/{Id}";
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UpgradeAccountValidator : Validator<UpgradeAccountRequest>
{
    public UpgradeAccountValidator()
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

public class UpgradeAccountResponse
{
    public Guid Id { get; set; }
    public PlayerDto Player { get; set; } = new();
}