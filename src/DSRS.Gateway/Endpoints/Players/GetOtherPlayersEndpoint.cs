using DSRS.Application.Features.Players.List;
using DSRS.Application.Features.Players.Queries;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Players;

public class GetOtherPlayersEndpoint(IMediator mediator) : Endpoint<GetOtherPlayersRequest, IResult>
{

    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetOtherPlayersRequest.Route);
        Policies("Authenticated", "RegisteredUser");
        Summary(s =>
        {
            s.Summary = "Retrieves list of players";
            s.Description = "Fetches list of players with queries like names etc.";
            s.ResponseExamples[200] = new { Id = "25598df5-6e11-45fb-975f-7cf85af872ea", Name = "John Doe" };
            // Document possible responses
            s.Responses[200] = "Players found and returned successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "Player with specified name not found";
            s.Responses[500] = "Internal server error occurred while processing the request.";
        });
        // Add tags for API grouping
        Tags("Players");
        // Add additional metadata
        Description(builder => builder
          .Accepts<GetOtherPlayersRequest>()
          .Produces<List<GetOtherPlayersResponse>>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetOtherPlayersRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOtherPlayersCommand(req.Query));

        return result.ToHttpResult(
          mapResponse => mapResponse.Select(p => new GetPlayersResponse(p.Id, p.Name)),
          locationBuilder => $"{GetOtherPlayersRequest.Route}",
          successStatusCode: StatusCodes.Status302Found);
    }

}

public class GetOtherPlayersRequest
{
    public const string Route = "players/others";

    public string? Query { get; set; } = string.Empty;
}

public record GetOtherPlayersResponse(Guid Id, string Name);