using ErrorOr;

namespace Web.API.Contracts.Responses;

internal sealed record ErrorDetails
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public List<ValidationError> ValidationErrors { get; init; } = [];
    public int StatusCode { get; init; }
    public Dictionary<string, object> AdditionalData { get; init; } = new();

    public static ErrorDetails Create(string code, string message, int statusCode, 
        Dictionary<string, object>? additionalData = null)
    {
        return new ErrorDetails
        {
            Code = code,
            Message = message,
            StatusCode = statusCode,
            AdditionalData = additionalData ?? new()
        };
    }

    public static ErrorDetails ValidationFailure(List<ValidationError> errors)
    {
        return new ErrorDetails
        {
            Code = "Validation.Failed",
            Message = "Um ou mais erros de validação ocorreram",
            ValidationErrors = errors,
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    public static ErrorDetails FromErrorOr(List<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        
        if (errors.Count == 0)
        {
            return Create("Unknown.Error", "Erro desconhecido", StatusCodes.Status500InternalServerError);
        }

        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var validationErrors = errors
                .Select(e => new ValidationError(e.Code, e.Description))
                .ToList();

            return ValidationFailure(validationErrors);
        }

        var primaryError = errors[0];
        var statusCode = MapErrorTypeToStatusCode(primaryError.Type);

        var additionalData = new Dictionary<string, object>();
        if (errors.Count > 1)
        {
            additionalData["additionalErrors"] = errors.Skip(1)
                .Select(e => new { e.Code, e.Description, Type = e.Type.ToString() })
                .ToList();
        }

        return Create(primaryError.Code, primaryError.Description, statusCode, additionalData);
    }

    private static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Failure => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };
}