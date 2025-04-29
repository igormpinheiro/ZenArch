using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models.Responses;

namespace Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController(ISender mediator) : ControllerBase
{
    private async Task<IActionResult> HandleRequest<TResponse>(IRequest<ErrorOr<TResponse>> request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        return result.Match(value => Ok(ApiResponse<TResponse>.Success(value)), ProcessErrors);
    }

    private IActionResult ProcessErrors(List<Error> errors)
    {
        var errorDetails = ErrorDetails.FromErrorOr(errors);

        return StatusCode(
            errorDetails.StatusCode,
            ApiResponse<object>.Failure(errorDetails));
    }

    protected async Task<IActionResult> SendCommand<TResponse>(IRequest<ErrorOr<TResponse>> request,
        CancellationToken cancellationToken)
    {
        return await HandleRequest(request, cancellationToken);
    }

    protected async Task<IActionResult> SendCommand(IRequest<ErrorOr<Success>> request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        return result.Match(_ => NoContent(), ProcessErrors);
    }
    
    protected async Task<IActionResult> SendCreateCommand<TResponse>(
        IRequest<ErrorOr<TResponse>> request, 
        string routeName,
        Func<TResponse, object> getRouteValuesFunc, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
    
        return result.Match(
            value => 
            {
                // Obter valores de rota a partir do resultado
                var routeValues = getRouteValuesFunc(value);
            
                // Gerar URI do recurso
                var resourceUri = Url.Link(routeName, routeValues);
            
                // Retornar 201 Created
                return Created(
                    resourceUri, 
                    ApiResponse<TResponse>.Success(value));
            },
            ProcessErrors
        );
    }

    protected async Task<IActionResult> SendQuery<TResponse>(IRequest<ErrorOr<TResponse>> request,
        CancellationToken cancellationToken)
    {
        return await HandleRequest(request, cancellationToken);
    }

    protected async Task<IActionResult> SendPaginatedQuery<TResponse>(
        IRequest<ErrorOr<(List<TResponse> items, int totalCount)>> request,
        PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        return result.Match(
            value =>
            {
                var paginatedResponse = PaginatedResponse<TResponse>.Create(
                    value.items,
                    paginationRequest,
                    value.totalCount);

                return Ok(ApiResponse<PaginatedResponse<TResponse>>.Success(paginatedResponse));
            },
            ProcessErrors
        );
    }
}
