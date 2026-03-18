using DSRS.Application.Features.Players.List;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;
using System;

namespace DSRS.Gateway.Endpoints.Players;

public class GetPlayersEndpoint(IMediator mediator) : Endpoint<GetPlayersRequest, IResult>
{

    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetPlayersRequest.Route);
        Policies("authenticated");
        Summary(s =>
        {
            s.Summary = "Retrieves list of players";
            s.Description = "Fetches list of players with queries like names etc.";
            s.ResponseExamples[200] = new { Id = "25598df5-6e11-45fb-975f-7cf85af872ea", Name = "John Doe" };
            // Document possible responses
            s.Responses[200] = "Players found and returned successfully";
            s.Responses[401] = "Authentication failed.";
            s.Responses[404] = "Player with specified name not found";
        });
        // Add tags for API grouping
        Tags("Players");
        // Add additional metadata
        Description(builder => builder
          .Accepts<GetPlayersRequest>()
          .Produces<GetPlayersResponse>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(401)
          .ProducesProblem(500));
    }

    public override async Task<IResult> ExecuteAsync(GetPlayersRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPlayersCommand(req.Query));

        return result.ToHttpResult(
          mapResponse => mapResponse.Select(p => new GetPlayersResponse(p.Id, p.Name)),
          locationBuilder => $"{GetPlayersRequest.Route}",
          successStatusCode: StatusCodes.Status302Found);
    }

}

public class GetPlayersRequest
{
    public const string Route = "players";

    public string? Query { get; set; } = string.Empty;
}

public record GetPlayersResponse(Guid Id, string Name);