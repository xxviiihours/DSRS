using DSRS.Application.Features.Market.Buy;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Market;

public sealed class BuyItemEndpoint(IMediator mediator) : Endpoint<BuyItemRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post(BuyItemRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Use to purchase a new item";
            // Document possible responses
            s.Responses[201] = "Item bought successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error";
        });

        // Add tags for API grouping
        Tags("Market");

        // Add additional metadata
        Description(builder => builder
          .Accepts<BuyItemRequest>("application/json")
          .Produces<BuyItemResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(BuyItemRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new BuyItemCommand(Guid.Parse(req.PlayerId), Guid.Parse(req.ItemId), req.Quantity), ct);

        return result.ToHttpResult(
             mapResponse => new BuyItemResponse(mapResponse.Id.ToString()),
             locationBuilder => $"{BuyItemRequest.Route}",
             successStatusCode: StatusCodes.Status201Created);

    }
}

public record BuyItemRequest()
{
    public const string Route = "/market/{playerId}";

    public static string BuildRoute(string playerId) => Route.Replace("{playerId}", playerId);

    [Required]
    public string PlayerId { get; set; } = string.Empty;
    [Required]
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
}

public class BuyItemResponse(string id)
{
    public string Id { get; set; } = id;
}

public class BuyItemValidator : Validator<BuyItemRequest>
{
    public BuyItemValidator()
    {
        RuleFor(x => x.PlayerId)
                 .NotEmpty()
                 .WithMessage("Invalid player id value.");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Invalid Item Id value.");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity cannot be less than zero.")
            .GreaterThan(0);
    }
}
