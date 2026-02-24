using DSRS.Application.Features.Dashboard;
using DSRS.Application.Features.Dashboard.Queries;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using FluentValidation;
using Mediator;
using System;

namespace DSRS.Gateway.Endpoints.Dashboard;

public class GetDailyPricesPerItemEndpoint(IMediator mediator) : Endpoint<GetDailyPricesPerItemRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetDailyPricesPerItemRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "retrieves daily prices based on item ID and player ID";
            s.Description = "Retrieves previous daily prices for a specific item";
            // Document possible responses
            s.Responses[200] = "Daily Prices found and returned successfully";
            s.Responses[404] = "Daily Price with specified IDs not found";
        });

        // Add tags for API grouping
        Tags("Dashboard");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetDailyPricesPerItemRequest>()
          .Produces<List<DashboardDto>>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }
    public override async Task<IResult> ExecuteAsync(GetDailyPricesPerItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDailyPricesPerItemCommand(
                Guid.Parse(request.ItemId), Guid.Parse(request.PlayerId)), cancellationToken);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{GetDailyPricesPerItemRequest.Route}",
            successStatusCode: StatusCodes.Status302Found);
    }
}

public class GetDailyPricesPerItemRequest
{
    public const string Route = "dashboard/{ItemId}/{PlayerId}";
    public string ItemId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
}
public class GetDailyPricesPerItemValidator : Validator<GetDailyPricesPerItemRequest>
{
    public GetDailyPricesPerItemValidator()
    {
        RuleFor(x => x.PlayerId)
          .NotEmpty()
          .WithMessage("Player ID is required.");
        RuleFor(x => x.ItemId)
          .NotEmpty()
          .WithMessage("Item ID is required.");
    }
}