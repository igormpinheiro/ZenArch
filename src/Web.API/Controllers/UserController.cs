using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly ISender _sender;


    public UserController(ILogger<UserController> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserViewModel>>> Get()
    {
        var result = await _sender.Send(new GetAllUsersQuery());

        if (result.IsError)
        {
            return BadRequest();
        }
            
        return Ok(result.Value.Select(user => new UserViewModel(user.Id.ToString("D"), user.Email, user.Name)).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserViewModel>> Get(Guid id)
    {
        var result = await _sender.Send(new GetUserByIdQuery(id));
        
        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }
        
        var response = new UserViewModel(result.Value.Id.ToString("D"), result.Value.Name, result.Value.Email);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserViewModel>> Post([FromBody] UserInputModel request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("user post");
        var command = new CreateUserCommand(request.Email, request.Name);
        
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }
        
        var response = new UserViewModel(result.Value.Id.ToString("D"), result.Value.Email, result.Value.Name);

        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }
}
