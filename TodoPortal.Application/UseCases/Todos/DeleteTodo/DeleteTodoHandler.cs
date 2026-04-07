using TodoPortal.Application.Common;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Todos.DeleteTodo;

public sealed class DeleteTodoHandler
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTodoHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task Handle(DeleteTodoCommand command, CancellationToken cancellationToken = default)
    {
        var deleted = await _todoRepository.DeleteAsync(command.Id, cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Todo", command.Id);
        }
    }
}
