using TodoPortal.Application.Common;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Users.GetUserTodos;

public sealed class GetUserTodosHandler
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;

    public GetUserTodosHandler(ITodoRepository todoRepository, IUserRepository userRepository)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<TodoDto>> Handle(GetUserTodosQuery query, CancellationToken cancellationToken = default)
    {
        if (!await _userRepository.ExistsAsync(query.UserId, cancellationToken))
        {
            throw new NotFoundException("User", query.UserId);
        }

        IReadOnlyCollection<Todo> todos = await _todoRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return todos.ToTodoDtos();
    }
}
