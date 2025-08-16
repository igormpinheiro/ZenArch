using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SharedKernel.Helpers;
using SharedKernel.Resources;
using Web.API.Contracts.Responses;

namespace Web.API.Middlewares;

public sealed class ErrorHandlingMiddleware(
    ILogger<ErrorHandlingMiddleware> logger,
    IWebHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {ExceptionType} - {Message}",
            exception.GetType().Name, exception.Message);

        httpContext.Response.ContentType = "application/json";

        var (statusCode, errorCode, errorMessage) = MapExceptionToError(exception);
        httpContext.Response.StatusCode = statusCode;

        var errorDetails = ErrorDetails.Create(errorCode, errorMessage, statusCode);

        // Em desenvolvimento, incluir detalhes técnicos
        if (environment.IsDevelopment())
        {
            errorDetails = errorDetails with
            {
                AdditionalData = new Dictionary<string, object>
                {
                    ["exceptionType"] = exception.GetType().Name,
                    ["stackTrace"] = exception.StackTrace ?? "No stack trace available",
                    ["innerException"] = exception.InnerException?.Message
                }
            };
        }

        var response = ApiResponse<object>.Failure(errorDetails);

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonHelper.DefaultOptions),
            cancellationToken);

        return true;
    }

    private static (int statusCode, string errorCode, string errorMessage) MapExceptionToError(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                "Argument.Null",
                ResourceMessages.ARGUMENT_NULL
            ),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Argument.Invalid",
                ResourceMessages.ARGUMENT_INVALID
            ),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Access.Unauthorized",
                ResourceMessages.ACCESS_UNAUTHORIZED
            ),
            NotImplementedException => (
                StatusCodes.Status501NotImplemented,
                "Feature.NotImplemented",
                ResourceMessages.FEATURE_NOT_IMPLEMENTED
            ),
            TimeoutException => (
                StatusCodes.Status408RequestTimeout,
                "Request.Timeout",
                ResourceMessages.REQUEST_TIMEOUT
            ),
            InvalidOperationException => (
                StatusCodes.Status422UnprocessableEntity,
                "Operation.Invalid",
                ResourceMessages.OPERATION_INVALID
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal.Server.Error",
                ResourceMessages.INTERNAL_SERVER_ERROR
            )
        };
    }
}
