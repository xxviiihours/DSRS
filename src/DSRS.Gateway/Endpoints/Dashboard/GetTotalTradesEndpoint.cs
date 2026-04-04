using DSRS.Application.Features.Dashboard;
using DSRS.Application.Features.Dashboard.GetTotalTrades;
using DSRS.Domain.ValueObjects;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Dashboard;

public class GetTotalTradesEndpoint(IMediator mediator) : Endpoint<GetTotalTradesRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetTotalTradesRequest.Route);
        Policies("Authenticated");
        Summary(s =>
        {
            s.Summary = "Retrieves total trades";
            s.Description = "Retrieves weekly total trades to use in chart datasets";
            // Document possible responses
            s.Responses[200] = "List of total trades found and returned successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "List total trades with specified ID not found";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Dashboard");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetTotalTradesRequest>()
          .Produces<List<TradeActivityDto>>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetTotalTradesRequest req, CancellationToken ct)
    {
        var id = PlayerId.From(Guid.Parse(req.Id));
        var result = await _mediator.Send(new GetTotalTradesCommand(id), ct);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => "",
            successStatusCode: StatusCodes.Status200OK);
    }
}

public class GetTotalTradesRequest
{
    public const string Route = "/dashboard/{Id}/total-trades";
    public string Id { get; set; } = string.Empty;
}
