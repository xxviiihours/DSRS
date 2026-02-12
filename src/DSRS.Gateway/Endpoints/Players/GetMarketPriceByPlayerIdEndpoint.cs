using DSRS.Application.Players.Get;
using DSRS.Gateway.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System;
using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Players;

public class GetMarketPriceByPlayerIdRequest
{
    public const string Route = "/market/{playerId}";
    public static string BuildRoute(string playerId) => Route.Replace("{playerId}", playerId);

    [Required]
    public string Id { get; set; } = string.Empty;
}

public class GetMarketPriceByPlayerIdValidator : Validator<GetMarketPriceByPlayerIdRequest>
{
    public GetMarketPriceByPlayerIdValidator()
    {
        RuleFor(x => x.Id)
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
            s.ExampleRequest = new GetMarketPriceByPlayerIdRequest { Id = "ac761785-ed42-11ce-dacb-00bdd0057645" };
            s.ResponseExamples[200] = new GetMarketPriceByPlayerIdResponse(1, "John Doe");

            // Document possible responses
            s.Responses[200] = "Player found and returned successfully";
            s.Responses[404] = "Player with specified ID not found";
        });

        // Add tags for API grouping
        Tags("Players");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetMarketPriceByPlayerIdRequest>("application/json")
          .Produces<GetMarketPriceByPlayerIdResponse>(201, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetMarketPriceByPlayerIdRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetMarketPriceCommand(Guid.Parse(request.Id!)), cancellationToken);


        return result.ToOkResult();
    }
}
