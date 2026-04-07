using Microsoft.AspNetCore.Mvc;
using TodoPortal.Api.Requests.Todos;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.UseCases.Todos.CreateTodo;
using TodoPortal.Application.UseCases.Todos.DeleteTodo;
using TodoPortal.Application.UseCases.Todos.GetTodoById;
using TodoPortal.Application.UseCases.Todos.GetTodos;
using TodoPortal.Application.UseCases.Todos.PatchTodo;
using TodoPortal.Application.UseCases.Todos.ReplaceTodo;

namespace TodoPortal.Api.Controllers;

[ApiController]
[Route("todos")]
public sealed class TodosController : ControllerBase
{
    private readonly GetTodosHandler _getTodosHandler;
    private readonly GetTodoByIdHandler _getTodoByIdHandler;
    private readonly CreateTodoHandler _createTodoHandler;
    private readonly ReplaceTodoHandler _replaceTodoHandler;
    private readonly PatchTodoHandler _patchTodoHandler;
    private readonly DeleteTodoHandler _deleteTodoHandler;

    public TodosController(
        GetTodosHandler getTodosHandler,
        GetTodoByIdHandler getTodoByIdHandler,
        CreateTodoHandler createTodoHandler,
        ReplaceTodoHandler replaceTodoHandler,
        PatchTodoHandler patchTodoHandler,
        DeleteTodoHandler deleteTodoHandler)
    {
        _getTodosHandler = getTodosHandler;
        _getTodoByIdHandler = getTodoByIdHandler;
        _createTodoHandler = createTodoHandler;
        _replaceTodoHandler = replaceTodoHandler;
        _patchTodoHandler = patchTodoHandler;
        _deleteTodoHandler = deleteTodoHandler;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TodoDto>>> GetTodos([FromQuery] int? userId, [FromQuery] bool? completed, CancellationToken cancellationToken)
    {
        var todos = await _getTodosHandler.Handle(new GetTodosQuery(userId, completed), cancellationToken);
        return Ok(todos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> GetTodoById(int id, CancellationToken cancellationToken)
    {
        var todo = await _getTodoByIdHandler.Handle(new GetTodoByIdQuery(id), cancellationToken);
        return Ok(todo);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TodoDto>> CreateTodo([FromBody] CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTodoCommand(request.UserId, request.Title, request.Completed);
        var created = await _createTodoHandler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(GetTodoById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> ReplaceTodo(int id, [FromBody] ReplaceTodoRequest request, CancellationToken cancellationToken)
    {
        var command = new ReplaceTodoCommand(id, request.UserId, request.Title, request.Completed);
        var updated = await _replaceTodoHandler.Handle(command, cancellationToken);
        return Ok(updated);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> PatchTodo(int id, [FromBody] PatchTodoRequest request, CancellationToken cancellationToken)
    {
        var command = new PatchTodoCommand(id, request.UserId, request.Title, request.Completed);
        var updated = await _patchTodoHandler.Handle(command, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTodo(int id, CancellationToken cancellationToken)
    {
        await _deleteTodoHandler.Handle(new DeleteTodoCommand(id), cancellationToken);
        return NoContent();
    }
}
