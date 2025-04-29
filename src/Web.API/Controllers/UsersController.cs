using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers;

public class UsersController(ISender mediator) : BaseApiController(mediator)
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return await SendQuery(new GetAllUsersQuery(), cancellationToken);
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await SendQuery(new GetUserByIdQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserInputModel request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request.Email, request.Name);

        return await SendCreateCommand(command, nameof(GetById), model => new { id = model.Id.ToString("D") },
            cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UserInputModel request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id, request.Email, request.Name);
        return await SendCommand(command, cancellationToken);
    }
}
