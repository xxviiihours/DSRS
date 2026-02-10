using DSRS.Application.Items.Create;
using DSRS.Gateway.Extensions;
using DSRS.SharedKernel.Primitives;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DSRS.Gateway.Endpoints.Items;

public class CreateItemRequest
{
    public const string Route = "/items";

    [Required]
    public string Name { get; set; } = String.Empty;
    public decimal BasePrice { get; set; } = 0;
    public decimal Volatility { get; set; } = 0;
}

public class CreateItemValidation : Validator<CreateItemRequest>
{
    public CreateItemValidation()
    {
        RuleFor(x => x.Name)
          .NotEmpty()
          .WithMessage("Name is required.")
          .MinimumLength(2)
          .MaximumLength(100);

        RuleFor(x => x.BasePrice)
            .NotEmpty()
            .WithMessage("Item price is required.")
            .GreaterThan(-1);

        RuleFor(x => x.Volatility)
            .NotEmpty()
            .WithMessage("Volatility cannot be less than 0.")
            .GreaterThan(0);
    }
}

public class CreateItemResponse(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
}

public class CreateItemEndpoint(IMediator mediator) : Endpoint<CreateItemRequest, IResult>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Post(CreateItemRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new item";
            // Document possible responses
            s.Responses[201] = "Item created successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error";
        });

        // Add tags for API grouping
        Tags("Players");

        // Add additional metadata
        Description(builder => builder
          .Accepts<CreateItemRequest>("application/json")
          .Produces<CreateItemResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(CreateItemRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateItemCommand(request.Name!, request.BasePrice, request.Volatility), ct);

        return result.ToCreatedResult($"{CreateItemRequest.Route}/{result.Data?.Id}");
    }
}
