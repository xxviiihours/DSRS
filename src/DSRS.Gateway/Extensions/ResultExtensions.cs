
using DSRS.SharedKernel.Primitives;
using FastEndpoints;
namespace DSRS.Gateway.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Returns 200 OK if success and no error, otherwise mapped error
    /// </summary>
    public static IResult ToOkResult<T>(this Result<T> result)
        => (result.IsSuccess && result.Error == null)
            ? TypedResults.Ok(result.Data)
            : result.MapErrorToIResult();

    /// <summary>
    /// Returns 201 Created if success and no error, otherwise mapped error
    /// </summary>
    public static IResult ToCreatedResult<T>(this Result<T> result, string location)
        => (result.IsSuccess && result.Error == null)
            ? TypedResults.Created(location, result.Data)
            : result.MapErrorToIResult();

    /// <summary>
    /// Returns 204 NoContent if success and no error, otherwise mapped error
    /// </summary>
    public static IResult ToNoContentResult<T>(this Result<T> result)
        => (result.IsSuccess && result.Error == null)
            ? TypedResults.NoContent()
            : result.MapErrorToIResult();

    /// <summary>
    /// Maps the domain Error to an appropriate IResult
    /// </summary>
    private static IResult MapErrorToIResult<T>(this Result<T> result)
    {
        if (result.Error == null)
        {
            // Fallback for unexpected state: no error but failure
            return TypedResults.Problem(
                title: "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return MapError(result.Error);
    }

    /// <summary>
    /// Converts domain error codes to HTTP status codes automatically using suffix convention
    /// </summary>
    private static IResult MapError(Error error)
    {
        int statusCode = error.Code switch
        {
            var c when c.EndsWith("NotFound") => StatusCodes.Status404NotFound,
            var c when c.EndsWith("Empty") => StatusCodes.Status400BadRequest,
            var c when c.EndsWith("Invalid") => StatusCodes.Status400BadRequest,
            var c when c.EndsWith("Exists") => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return statusCode switch
        {
            StatusCodes.Status400BadRequest => TypedResults.BadRequest(error.Message),
            StatusCodes.Status404NotFound => TypedResults.NotFound(error.Message),
            StatusCodes.Status409Conflict => TypedResults.Conflict(error.Message),
            _ => TypedResults.Problem(
                    title: "Internal Server Error",
                    detail: error.Message,
                    statusCode: statusCode)
        };
    }
}