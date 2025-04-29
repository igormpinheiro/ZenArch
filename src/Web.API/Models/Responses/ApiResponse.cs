using System.Diagnostics;
using ErrorOr;

namespace Web.API.Models.Responses;

// Classe base para todas as respostas da API
internal class ApiResponse<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public ErrorDetails? Error { get; }
    public string TraceId { get; }
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    private ApiResponse(bool isSuccess, T? data, ErrorDetails? error, string? traceId)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        TraceId = traceId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }

    public static ApiResponse<T> Success(T data, string traceId = null)
    {
        return new ApiResponse<T>(true, data, null, traceId);
    }

    public static ApiResponse<T> Failure(ErrorDetails error, string traceId = null)
    {
        return new ApiResponse<T>(false, default, error, traceId);
    }
}

// Detalhes de erro padronizados
internal class ErrorDetails
{
    public string Code { get; }
    public string Message { get; }
    public List<ValidationError>? ValidationErrors { get; }
    public int StatusCode { get; }

    private ErrorDetails(string code, string message, List<ValidationError>? validationErrors, int statusCode)
    {
        Code = code;
        Message = message;
        ValidationErrors = validationErrors;
        StatusCode = statusCode;
    }

    public static ErrorDetails Create(string code, string message, int statusCode)
    {
        return new ErrorDetails(code, message, null, statusCode);
    }

    public static ErrorDetails ValidationFailure(List<ValidationError> errors)
    {
        return new ErrorDetails("Validation.Error", "Ocorreram erros de validação", errors,
            StatusCodes.Status400BadRequest);
    }

    public static ErrorDetails FromErrorOr(List<Error> errors)
    {
        var firstError = errors[0];

        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var validationErrors = errors
                .Select(e => new ValidationError(e.Code, e.Description))
                .ToList();

            return ValidationFailure(validationErrors);
        }

        // Mapeia o tipo de erro para um código HTTP
        var statusCode = firstError.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Create(firstError.Code, firstError.Description, statusCode);
    }
}

// Erro de validação
internal class ValidationError
{
    public string PropertyName { get; }
    public string Message { get; }

    public ValidationError(string propertyName, string message)
    {
        PropertyName = propertyName;
        Message = message;
    }
}