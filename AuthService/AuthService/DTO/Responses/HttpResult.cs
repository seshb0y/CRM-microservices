namespace ControllerFirst.DTO.Responses;

public class HttpResult<T>
{
    public HttpResult(bool isSuccess, T? data, string? message = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }

    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public T Data { get; init; }

    public static HttpResult<T> Success(T data, string? message = null) => new(true, data, message);

    public static HttpResult<T> Error(T? data, string message) => new(false, data, message);
}