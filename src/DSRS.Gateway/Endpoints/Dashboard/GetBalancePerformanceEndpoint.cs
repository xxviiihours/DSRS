using DSRS.Application.Features.Dashboard;
using DSRS.Application.Features.Dashboard.GetBalancePerformance;
using DSRS.Domain.ValueObjects;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Dashboard;

public class GetBalancePerformanceEndpoint(IMediator mediator) : Endpoint<GetBalancePerformanceRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetBalancePerformanceRequest.Route);
        Policies("Authenticated");
        Summary(s =>
        {
            s.Summary = "Retrieves performance based on balance changes";
            s.Description = "Retrieves performance based on balance changes by playerID";
            // Document possible responses
            s.Responses[200] = "Performance balance found and returned successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "Performance balance with specified IDs not found";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });

        // Add tags for API grouping
        Tags("Dashboard");

        // Add additional metadata
        Description(builder => builder
          .Accepts<GetBalancePerformanceRequest>()
          .Produces<List<BalancePerformanceDto>>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetBalancePerformanceRequest req, CancellationToken ct)
    {
        var id = PlayerId.From(Guid.Parse(req.Id));
        var result = await _mediator.Send(new GetBalancePerformanceCommand(id), ct);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{GetBalancePerformanceRequest.Route}",
            successStatusCode: StatusCodes.Status200OK);
    }
}
public class GetBalancePerformanceRequest
{
    public const string Route = "/dashboard/{id}/performance";
    public string Id { get; set; } = string.Empty;
}