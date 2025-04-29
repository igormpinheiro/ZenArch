using Microsoft.AspNetCore.Diagnostics;
using Web.API.Models.Responses;

namespace Web.API.Middlewares;

public sealed class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var errorCode = "Internal.ServerError";
        var errorMessage = "Ocorreu um erro interno no servidor.";

        if (httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            errorMessage = exception.Message;
            errorCode = exception.GetType().Name;
        }

        var errorDetails = ErrorDetails.Create(
            errorCode,
            errorMessage,
            StatusCodes.Status500InternalServerError);

        var response = ApiResponse<object>.Failure(errorDetails);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
