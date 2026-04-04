using DSRS.Application.Features.Dashboard;
using DSRS.Application.Features.Dashboard.GetTradeActivities;
using DSRS.Domain.ValueObjects;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Dashboard;

public class GetTradeActivitiesEndpoint(IMediator mediator) : Endpoint<GetTradeActivitiesRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetTradeActivitiesRequest.Route);
        Policies("Authenticated");
        Summary(s =>
        {
            s.Summary = "Retrieves recent trade activies";
            s.Description = "Retrieves recent trade activities based on the player id";
            // Document possible responses
            s.Responses[200] = "Recent trade activities found and returned successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "Recent trade activities with specified ID not found";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Dashboard");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetTradeActivitiesRequest>()
          .Produces<List<TradeActivityDto>>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetTradeActivitiesRequest request, CancellationToken cancellationToken)
    {
        var id = PlayerId.From(Guid.Parse(request.Id));

        var result = await _mediator.Send(
            new GetTradeActivitiesCommand(id), cancellationToken);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{GetDailyPricesPerItemRequest.Route}",
            successStatusCode: StatusCodes.Status302Found);
    }
}

public class GetTradeActivitiesRequest
{
    public const string Route = "/dashboard/{Id}/trade-activities";
    public string Id { get; set; } = string.Empty;
}
