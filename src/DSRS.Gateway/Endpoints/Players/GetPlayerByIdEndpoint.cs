using DSRS.Application.Features.Players.Get;
using DSRS.Gateway.Common.Extensions;
using FastEndpoints;
using Mediator;
using System;

namespace DSRS.Gateway.Endpoints.Players;

public class GetPlayerByIdEndpoint(IMediator mediator) : Endpoint<GetPlayerByIdRequest, IResult>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetPlayerByIdRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Retrieves player information by Id";
            s.Description = "Fetches player details based on the provided Id.";
            s.ResponseExamples[200] = new { Id = "25598df5-6e11-45fb-975f-7cf85af872ea", Name = "John Doe" };
            // Document possible responses
            s.Responses[200] = "Player found and returned successfully";
            s.Responses[404] = "Player with specified name not found";
        });
        // Add tags for API grouping
        Tags("Players");
        // Add additional metadata
        Description(builder => builder
          .Accepts<GetPlayerByIdRequest>()
          .Produces<GetPlayerByIdResponse>(200, "application/json")
          .ProducesProblem(400)
          .ProducesProblem(500));
    }


    public override async Task<IResult> ExecuteAsync(GetPlayerByIdRequest request, CancellationToken cancellationToken)
    {
        // Simulate fetching player data by name (replace with actual data retrieval logic)
        var result = await _mediator.Send(new GetPlayerByIdCommand(Guid.Parse(request.Id)), cancellationToken);

        return result.ToHttpResult(
            mapResponse => mapResponse,
            locationBuilder => $"{GetPlayerByIdRequest.Route}",
            successStatusCode: StatusCodes.Status302Found); ;
    }
}

public class GetPlayerByIdRequest
{
    public const string Route = "/players/{id}";
    public string Id { get; set; } = string.Empty;
}

public class GetPlayerByIdResponse(Guid id, string name, decimal balance)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public decimal Balance { get; set; } = balance;
}
