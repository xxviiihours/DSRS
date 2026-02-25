using DSRS.Application.Features.Market.Sell;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Market;

public sealed class SellItemEndpoint(IMediator mediator) : Endpoint<SellItemRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Put(SellItemRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Use to sell an item in the inventory";
            // Document possible responses
            s.Responses[200] = "Item sold successfully";
            s.Responses[400] = "Invalid input data - validation errors";
            s.Responses[500] = "Internal server error";
        });

        // Add tags for API grouping
        Tags("Market");

        // Add additional metadata
        Description(builder => builder
          .Accepts<BuyItemRequest>("application/json")
          .Produces<BuyItemResponse>(204, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(SellItemRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SellItemCommand(Guid.Parse(req.PlayerId), Guid.Parse(req.ItemId), req.Quantity), ct);

        return result.ToHttpResult(
             mapResponse => new SellItemResponse(mapResponse.Id.ToString()),
             locationBuilder => $"{SellItemRequest.Route}");

    }
}

public class SellItemRequest
{
    public const string Route = "/market/{playerId}";
    public static string BuildRoute(string playerId) => Route.Replace("{playerId}", playerId);

    [Required]
    public string PlayerId { get; set; } = string.Empty;
    [Required]
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
}

public record SellItemResponse(string Id)
{

    public string Id { get; set; } = Id;
}

public class SellItemValidator : Validator<SellItemRequest>
{
    public SellItemValidator()
    {
        RuleFor(p => p.PlayerId)
            .NotEmpty()
            .WithMessage("Player id cannot be empty.");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Invalid Item Id value.");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity cannot be less than zero.")
            .GreaterThan(0);
    }
}