using DSRS.Application.Features.Market.GetMarketPrices;
using DSRS.Domain.ValueObjects;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Market;

public class GetMarketPricesEndpoint(IMediator mediator) :
    Endpoint<GetMarketPricesRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetMarketPricesRequest.Route);
        Policies("Authenticated");
        Summary(s =>
        {
            s.Summary = "Generates daily prices based on player ID";
            s.Description = "Retrieves daily prices for each items in the market";

            // Document possible responses
            s.Responses[200] = "Player found and returned successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "Player with specified ID not found";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Market");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetMarketPricesRequest>()
          .Produces<GetMarketPricesResponse>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetMarketPricesRequest request, CancellationToken cancellationToken)
    {
        var id = PlayerId.From(Guid.Parse(request.PlayerId));
        var result = await _mediator.Send(
            new GetMarketPricesCommand(id), cancellationToken);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{GetMarketPricesRequest.Route}",
            successStatusCode: StatusCodes.Status302Found);
    }
}

public class GetMarketPricesRequest
{
    public const string Route = "/market/{playerId}";

    [Required]
    public string PlayerId { get; set; } = string.Empty;
}

public class GetMarketPricesValidator : Validator<GetMarketPricesRequest>
{
    public GetMarketPricesValidator()
    {
        RuleFor(x => x.PlayerId)
          .NotEmpty()
          .WithMessage("Player ID is required.");
    }
}

public sealed record GetMarketPricesResponse(string Id, string Name)
{
    public string Id { get; set; } = Id;
    public string Name { get; set; } = Name;
}