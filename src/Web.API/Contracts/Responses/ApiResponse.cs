using System.Diagnostics;

namespace Web.API.Contracts.Responses;

internal sealed record ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ErrorDetails? Error { get; init; }
    public string TraceId { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    private ApiResponse() { }

    public static ApiResponse<T> Success(T data, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Error = null,
            TraceId = traceId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString("N"),
        };
    }

    public static ApiResponse<T> Failure(ErrorDetails error, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Data = default,
            Error = error,
            TraceId = traceId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString("N")
        };
    }
}