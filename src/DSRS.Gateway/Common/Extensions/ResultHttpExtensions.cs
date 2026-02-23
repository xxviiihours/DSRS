using DSRS.SharedKernel.Primitives;
namespace DSRS.Gateway.Common.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult<TDomain, TResponse>(
        this Result<TDomain> result,
        Func<TDomain, TResponse> mapResponse,
        Func<TDomain, string>? locationBuilder = null,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return successStatusCode switch
            {
                StatusCodes.Status201Created =>
                    TypedResults.Created(
                        locationBuilder!(result.Data!),
                        mapResponse(result.Data!)
                    ),

                StatusCodes.Status204NoContent =>
                    TypedResults.NoContent(),

                StatusCodes.Status302Found =>
                    TypedResults.Ok(
                        mapResponse(result.Data!)
                    ),
                _ =>
                TypedResults.Ok(
                    mapResponse(result.Data!)
                )
            };
        }

        return MapError(result.Error);
    }

    private static IResult MapError(Error? error)
    {
        if (error is null)
        {
            return TypedResults.Problem(
                title: "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return error.Code switch
        {
            var c when c.EndsWith("NotFound") =>
                TypedResults.Problem(
                    title: "Not Found",
                    detail: error.Message,
                    statusCode: StatusCodes.Status404NotFound),

            var c when c.EndsWith("Invalid") ||
                c.EndsWith("Empty") ||
                c.EndsWith("Insufficient") =>
                    TypedResults.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            { error.Code, new[] { error.Message } }
                        }),

            var c when c.EndsWith("Exists") =>
                TypedResults.Problem(
                    title: "Conflict",
                    detail: error.Message,
                    statusCode: StatusCodes.Status409Conflict),

            _ => TypedResults.Problem(
                    title: "Request failed",
                    detail: error.Message,
                    statusCode: StatusCodes.Status400BadRequest)
        };
    }
}