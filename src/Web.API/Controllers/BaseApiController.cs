using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Pagination;
using Web.API.Contracts.Responses;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController(ISender mediator, ILogger<BaseApiController> logger) : ControllerBase
{
    private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<BaseApiController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private async Task<IActionResult> HandleRequest<TResponse>(
        IRequest<ErrorOr<TResponse>> request,
        CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("Processing request {RequestType}", typeof(TResponse).Name);

        var result = await _mediator.Send(request, cancellationToken);

        return result.Match(
            value =>
            {
                _logger.LogDebug("Request processed successfully");
                return Ok(CreateSuccessResponse(value));
            },
            errors =>
            {
                _logger.LogWarning("Request failed with errors: {@Errors}", errors);
                return ProcessErrors(errors);
            }
        );
    }

    private async Task<IActionResult> HandleVoidRequest(
        IRequest<ErrorOr<Success>> request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.Match(
            _ =>
            {
                _logger.LogDebug("Void request processed successfully");
                return NoContent();
            },
            errors =>
            {
                _logger.LogWarning("Void request failed with errors: {@Errors}", errors);
                return ProcessErrors(errors);
            }
        );
    }

    private async Task<IActionResult> HandleCreateRequest<TResponse>(
        IRequest<ErrorOr<TResponse>> request,
        string routeName,
        Func<TResponse, object> getRouteValues,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.Match(
            value =>
            {
                var routeValues = getRouteValues(value);
                var resourceUri = Url.Link(routeName, routeValues);

                _logger.LogDebug("Resource created successfully at {ResourceUri}", resourceUri);

                return Created(resourceUri, CreateSuccessResponse(value));
            },
            errors =>
            {
                _logger.LogWarning("Create request failed with errors: {@Errors}", errors);
                return ProcessErrors(errors);
            }
        );
    }

    // ===== MÉTODO CORRIGIDO PARA PAGINATEDRESULT =====
    private async Task<IActionResult> HandlePaginatedRequest<TResponse>(
        IRequest<ErrorOr<PaginatedResult<TResponse>>> request,
        CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("Processing paginated request {RequestType}", typeof(TResponse).Name);

        var result = await _mediator.Send(request, cancellationToken);

        return result.Match(
            paginatedResult =>
            {
                _logger.LogDebug(
                    "Paginated request processed successfully. Items: {ItemCount}, Total: {TotalCount}",
                    paginatedResult.Metadata.CurrentPageCount,
                    paginatedResult.Metadata.TotalCount);

                return Ok(CreateSuccessResponse(paginatedResult));
            },
            errors =>
            {
                _logger.LogWarning("Paginated request failed with errors: {@Errors}", errors);
                return ProcessErrors(errors);
            }
        );
    }

    private IActionResult ProcessErrors(List<Error> errors)
    {
        var errorDetails = ErrorDetails.FromErrorOr(errors);
        return StatusCode(errorDetails.StatusCode, ApiResponse<object>.Failure(errorDetails));
    }

    private static ApiResponse<T> CreateSuccessResponse<T>(T data)
    {
        return ApiResponse<T>.Success(data);
    }

    // Métodos convenientes para backward compatibility e clareza
    protected Task<IActionResult> SendCommand<TResponse>(
        IRequest<ErrorOr<TResponse>> request,
        CancellationToken cancellationToken) =>
        HandleRequest(request, cancellationToken);

    protected Task<IActionResult> SendCommand(
        IRequest<ErrorOr<Success>> request,
        CancellationToken cancellationToken) =>
        HandleVoidRequest(request, cancellationToken);

    protected Task<IActionResult> SendQuery<TResponse>(
        IRequest<ErrorOr<TResponse>> request,
        CancellationToken cancellationToken) =>
        HandleRequest(request, cancellationToken);

    protected Task<IActionResult> SendCreateCommand<TResponse>(
        IRequest<ErrorOr<TResponse>> request,
        string routeName,
        Func<TResponse, object> getRouteValues,
        CancellationToken cancellationToken) =>
        HandleCreateRequest(request, routeName, getRouteValues, cancellationToken);

    protected Task<IActionResult> SendPaginatedQuery<TResponse>(
        IRequest<ErrorOr<PaginatedResult<TResponse>>> request,
        CancellationToken cancellationToken) =>
        HandlePaginatedRequest(request, cancellationToken);
}
