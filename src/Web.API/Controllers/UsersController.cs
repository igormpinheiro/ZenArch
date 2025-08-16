using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Mappings;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Pagination;
using Web.API.Contracts.Responses;

namespace Web.API.Controllers;

[Tags("Users")]
public sealed class UsersController(ISender mediator, ILogger<UsersController> logger)
    : BaseApiController(mediator, logger)
{
    /// <summary>
    /// Recupera todos os usuários com paginação
    /// </summary>
    /// <param name="pagination">Parâmetros de paginação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários</returns>
    [HttpGet]
    [ProducesResponseType<ApiResponse<PaginatedResult<UserViewModel>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        // Converter para o modelo do SharedKernel
        var sharedKernelPagination = SharedKernel.Common.Pagination.PaginationRequest.Create(
            pagination.Page,
            pagination.PageSize,
            pagination.SortBy,
            pagination.SortDescending);

        var query = new GetAllUsersQuery(sharedKernelPagination);

        // Agora funciona corretamente!
        return await SendPaginatedQuery<UserViewModel>(query, cancellationToken);
    }

    /// <summary>
    /// Recupera um usuário específico por ID
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType<ApiResponse<UserViewModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(id);
        return await SendQuery(query, cancellationToken);
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    [HttpPost]
    [ProducesResponseType<ApiResponse<UserViewModel>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] UserInputModel request,
        CancellationToken cancellationToken = default)
    {
        var command = request.ToCreateCommand();
        return await SendCreateCommand(
            command,
            nameof(GetById),
            response => new { id = response.Id },
            cancellationToken);
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UserInputModel request,
        CancellationToken cancellationToken = default)
    {
        var command = request.ToUpdateCommand(id);
        return await SendCommand(command, cancellationToken);
    }

    /// <summary>
    /// Remove um usuário
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiResponse<object>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserCommand(id);
        return await SendCommand(command, cancellationToken);
    }
}
