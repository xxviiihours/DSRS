namespace DSRS.SharedKernel.Primitives;

public sealed record Error(string Code, string Message);
public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}
public sealed class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool success, T? value, Error? error)
        : base(success, error)
    {
        Data = value;
    }

    public static Result<T> Success(T value)
        => new(true, value, null);

    public static new Result<T> Failure(Error error)
        => new(false, default, error);
}