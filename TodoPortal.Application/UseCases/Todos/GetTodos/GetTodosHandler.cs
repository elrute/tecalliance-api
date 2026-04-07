using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Todos.GetTodos;

public sealed class GetTodosHandler
{
    private readonly ITodoRepository _todoRepository;

    public GetTodosHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<IReadOnlyCollection<TodoDto>> Handle(GetTodosQuery query, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Todo> todos;

        if (query.HasFilters)
        {
            todos = await _todoRepository.FindAsync(query.UserId, query.Completed, cancellationToken);
        }
        else
        {
            todos = await _todoRepository.GetAllAsync(cancellationToken);
        }

        return todos.ToTodoDtos();
    }
}
