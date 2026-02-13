using DSRS.Application.Features.Players.Create;
using DSRS.Gateway.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Players;

public class CreatePlayerRequest
{
    public const string Route = "/players";

    [Required]
    public string Name { get; set; } = String.Empty;
    public decimal Balance { get; set; } = 0;
}

public class CreatePlayerValidator : Validator<CreatePlayerRequest>
{
    public CreatePlayerValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Name is required.")
          .MinimumLength(2)
          .MaximumLength(100);
    }
}

public class CreatePlayerResponse(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}

public class CreatePlayerEndpoint(IMediator mediator) : Endpoint<CreatePlayerRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post(CreatePlayerRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new player";
            // Document possible responses
            s.Responses[201] = "Player created successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error";
        });

        // Add tags for API grouping
        Tags("Players");

        // Add additional metadata
        Description(builder => builder
          .Accepts<CreatePlayerRequest>("application/json")
          .Produces<CreatePlayerResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(CreatePlayerRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreatePlayerCommand(request.Name!, request.Balance), cancellationToken);
        
        return result.ToCreatedResult($"{CreatePlayerRequest.Route}/{result.Data?.Id}");
    }
}
