using TodoPortal.Application.Common;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Todos.PatchTodo;

public sealed class PatchTodoHandler
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;

    public PatchTodoHandler(ITodoRepository todoRepository, IUserRepository userRepository)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
    }

    public async Task<TodoDto> Handle(PatchTodoCommand command, CancellationToken cancellationToken = default)
    {
        if (command.UserId is null && command.Title is null && command.Completed is null)
        {
            throw ValidationException.Single("payload", "At least one field must be provided.");
        }

        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (command.Title is not null && string.IsNullOrWhiteSpace(command.Title))
        {
            AddError(errors, "title", "Title cannot be empty.");
        }

        if (command.UserId is int userId)
        {
            if (!await _userRepository.ExistsAsync(userId, cancellationToken))
            {
                AddError(errors, "userId", $"User '{userId}' does not exist.");
            }
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(ToPayload(errors));
        }

        var todo = await _todoRepository.GetByIdAsync(command.Id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException("Todo", command.Id);
        }

        if (command.UserId.HasValue)
        {
            todo.UserId = command.UserId.Value;
        }

        if (command.Title is string title)
        {
            todo.Title = title.Trim();
        }

        if (command.Completed.HasValue)
        {
            todo.Completed = command.Completed.Value;
        }

        var updated = await _todoRepository.UpdateAsync(todo, cancellationToken);
        return updated.ToDto();
    }

    private static void AddError(IDictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.TryGetValue(key, out var list))
        {
            list = new List<string>();
            errors[key] = list;
        }

        list.Add(message);
    }

    private static Dictionary<string, string[]> ToPayload(Dictionary<string, List<string>> source)
        => source.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
}
