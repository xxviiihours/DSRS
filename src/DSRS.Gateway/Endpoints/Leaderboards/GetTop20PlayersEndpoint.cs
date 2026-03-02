using DSRS.Application.Features.Leaderboards;
using DSRS.Application.Features.Leaderboards.Queries;
using DSRS.Application.Features.Players.Get;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;
using System;

namespace DSRS.Gateway.Endpoints.Leaderboards;

public class GetTop20PlayersEndpoint(IMediator mediator) : Endpoint<GetTop20PlayersRequest, IResult>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(GetTop20PlayersRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Retrieves top 20 players";
      s.Description = "Retrieves top 20 players including the user based on player Id";
      // Document possible responses
      s.Responses[200] = "List found and returned successfully";
      s.Responses[404] = "Unable to find top 20 players";
    });

    // Add tags for API grouping
    Tags("Leaderboards");

    // Add additional metadata
    Description(builder => builder
      .Accepts<GetTop20PlayersRequest>()
      .Produces<List<PlayerLeaderboardDto>>(200, "application/json")
      .ProducesProblem(400)
      .ProducesProblem(500));
  }

  public override async Task<IResult> ExecuteAsync(GetTop20PlayersRequest request, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetTop20PlayersCommand(Guid.Parse(request.Id)), ct);

    return result.ToHttpResult(mapResponse => mapResponse,
            locationBuilder => $"{GetTop20PlayersRequest.Route}",
            successStatusCode: StatusCodes.Status302Found);
  }
}
public class GetTop20PlayersRequest
{
  public const string Route = "/leaderboards/top20/{Id}";
  public string Id { get; set; } = string.Empty;
}