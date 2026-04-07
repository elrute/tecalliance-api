using Microsoft.AspNetCore.Mvc;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.UseCases.Users.GetUserById;
using TodoPortal.Application.UseCases.Users.GetUserTodos;
using TodoPortal.Application.UseCases.Users.GetUsers;

namespace TodoPortal.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private readonly GetUsersHandler _getUsersHandler;
    private readonly GetUserByIdHandler _getUserByIdHandler;
    private readonly GetUserTodosHandler _getUserTodosHandler;

    public UsersController(
        GetUsersHandler getUsersHandler,
        GetUserByIdHandler getUserByIdHandler,
        GetUserTodosHandler getUserTodosHandler)
    {
        _getUsersHandler = getUsersHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _getUserTodosHandler = getUserTodosHandler;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetUsers([FromQuery] string? email, [FromQuery] string? username, CancellationToken cancellationToken)
    {
        var users = await _getUsersHandler.Handle(new GetUsersQuery(email, username), cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await _getUserByIdHandler.Handle(new GetUserByIdQuery(id), cancellationToken);
        return Ok(user);
    }

    [HttpGet("{id:int}/todos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<TodoDto>>> GetUserTodos(int id, CancellationToken cancellationToken)
    {
        var todos = await _getUserTodosHandler.Handle(new GetUserTodosQuery(id), cancellationToken);
        return Ok(todos);
    }
}
