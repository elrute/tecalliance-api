using TodoPortal.Application.Common;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Todos.GetTodoById;

public sealed class GetTodoByIdHandler
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoByIdHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoDto> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.GetByIdAsync(query.Id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException("Todo", query.Id);
        }

        return todo.ToDto();
    }
}
