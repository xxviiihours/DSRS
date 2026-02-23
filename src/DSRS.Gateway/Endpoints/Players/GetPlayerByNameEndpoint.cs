using DSRS.Application.Features.Players.Get;
using DSRS.Gateway.Common.Extensions;
using DSRS.SharedKernel.Primitives;
using FastEndpoints;
using Mediator;

namespace DSRS.Gateway.Endpoints.Players;

public class GetPlayerByNameEndpoint(IMediator mediator) : Endpoint<GetPlayerByNameRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetPlayerByNameRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Retrieves player information by name";
            s.Description = "Fetches player details based on the provided name.";
            s.ResponseExamples[200] = new { Id = "25598df5-6e11-45fb-975f-7cf85af872ea", Name = "John Doe" };
            // Document possible responses
            s.Responses[200] = "Player found and returned successfully";
            s.Responses[404] = "Player with specified name not found";
        });
        // Add tags for API grouping
        Tags("Players");
        // Add additional metadata
        Description(builder => builder
          .Accepts<GetPlayerByNameRequest>()
          .Produces<GetPlayerByNameResponse>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }


    public override async Task<IResult> ExecuteAsync(GetPlayerByNameRequest request, CancellationToken cancellationToken)
    {
        // Simulate fetching player data by name (replace with actual data retrieval logic)
        var result = await _mediator.Send(new GetPlayerByNameCommand(request.Name), cancellationToken);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{GetPlayerByNameRequest.Route}",
            successStatusCode: StatusCodes.Status302Found); ;
    }
}

public class GetPlayerByNameRequest
{
    public const string Route = "/players/name/{name}";
    public string Name { get; set; } = string.Empty;
}

public class GetPlayerByNameResponse(Guid id, string name, decimal balance)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public decimal Balance { get; set; } = balance;
}
