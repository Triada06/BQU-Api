namespace BGU.Application.Common;

public class ApiResult<T>
{
    public T? Data { get; set; }
    public string Message { get; set; }
    public bool IsSucceeded { get; set; }
    public int StatusCode { get; set; }

    // Static factory methods for convenience
    public static ApiResult<T> Success(T data, string message = "Success")
        => new()
        {
            Data = data,
            Message = message,
            IsSucceeded = true,
            StatusCode = 200
        };

    public static ApiResult<T> NotFound(string message = "Not Found")
        => new()
        {
            Data = default,
            Message = message,
            IsSucceeded = false,
            StatusCode = 404
        };

    public static ApiResult<T> BadRequest(T data, string message = "Bad Request")
        => new()
        {
            Data = default,
            Message = message,
            IsSucceeded = false,
            StatusCode = 400
        };

    public static ApiResult<T> Unauthorized(string message = "Unauthorized")
        => new()
        {
            Data = default,
            Message = message,
            IsSucceeded = false,
            StatusCode = 401
        };

    public static ApiResult<T> SystemError(string message = "Internal Server Error")
        => new()
        {
            Data = default,
            Message = message,
            IsSucceeded = false,
            StatusCode = 500
        };
}