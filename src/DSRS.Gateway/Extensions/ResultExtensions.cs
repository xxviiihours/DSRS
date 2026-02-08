
using DSRS.SharedKernel.Primitives;
using FastEndpoints;
namespace DSRS.Gateway.Extensions;

public static class ResultExtensions
{

    public static IResult ToOkResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Data);
        }

        return MapError(result.Error!);
    }
    public static IResult ToCreatedResult<T>(this Result<T> result, string location)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Created(location, result.Data);
        }

        return MapError(result.Error!);
    }

    private static IResult MapError(Error error)
        => error.Code switch
        {
            "Player.Name.Exists" => TypedResults.Conflict(error.Message),
            "Player.Name.Empty" => TypedResults.BadRequest(error.Message),
            "Player.NotFound" => TypedResults.NotFound(error.Message),
            "Player.Invalid" => TypedResults.BadRequest(error.Message),
            "Player.Empty" => TypedResults.BadRequest(error.Message),

            _ => TypedResults.Problem(
                    title: "Internal Server Error",
                    detail: error.Message,
                    statusCode: StatusCodes.Status500InternalServerError)
        };
}
