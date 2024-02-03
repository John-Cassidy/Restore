namespace Restore.Core.Results;

public class Result<T>
{
    public T Value { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public int StatusCode { get; set; } = 200; // optimistic default

    public static Result<T> Success(T value) => new Result<T> { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error, int statusCode = 400) => new Result<T> { ErrorMessage = error, IsSuccess = false, StatusCode = statusCode };
}