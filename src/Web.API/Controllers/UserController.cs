using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ISender mediator) : ApiController(mediator)
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return await Send(new GetAllUsersQuery(), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        return await Send(new GetUserByIdQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserInputModel request, CancellationToken cancellationToken)
    {
       
        var command = new CreateUserCommand(request.Email, request.Name);

        return await Send(command, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UserInputModel request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id, request.Email, request.Name);
        return await Send(command, cancellationToken);
    }
}
