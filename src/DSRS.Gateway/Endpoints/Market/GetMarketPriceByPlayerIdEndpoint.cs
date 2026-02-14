using DSRS.Application.Features.Players.Get;
using DSRS.Gateway.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Market;

public class GetMarketPriceByPlayerIdRequest
{
    public const string Route = "/market/{playerId}";

    [Required]
    public string PlayerId { get; set; } = string.Empty;
}

public class GetMarketPriceByPlayerIdValidator : Validator<GetMarketPriceByPlayerIdRequest>
{
    public GetMarketPriceByPlayerIdValidator()
    {
        RuleFor(x => x.PlayerId)
          .NotEmpty()
          .WithMessage("Player ID is required.");
    }
}

public sealed record GetMarketPriceByPlayerIdResponse(int Id, string Name)
{
}
public class GetMarketPriceByPlayerIdEndpoint(IMediator mediator) : Endpoint<GetMarketPriceByPlayerIdRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetMarketPriceByPlayerIdRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Generates daily prices based on player ID";
            s.Description = "Retrieves daily prices for each items in the market";
            s.ResponseExamples[200] = new GetMarketPriceByPlayerIdResponse(1, "John Doe");

            // Document possible responses
            s.Responses[200] = "Player found and returned successfully";
            s.Responses[404] = "Player with specified ID not found";
        });

        // Add tags for API grouping
        Tags("Market");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetMarketPriceByPlayerIdRequest>("application/json")
          .Produces<GetMarketPriceByPlayerIdResponse>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetMarketPriceByPlayerIdRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetMarketPriceCommand(Guid.Parse(request.PlayerId)), cancellationToken);

        return result.ToOkResult();
    }
}
