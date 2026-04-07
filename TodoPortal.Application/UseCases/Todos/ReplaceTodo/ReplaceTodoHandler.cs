using TodoPortal.Application.Common;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Todos.ReplaceTodo;

public sealed class ReplaceTodoHandler
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;

    public ReplaceTodoHandler(ITodoRepository todoRepository, IUserRepository userRepository)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
    }

    public async Task<TodoDto> Handle(ReplaceTodoCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            AddError(errors, "title", "Title is required.");
        }

        if (!await _userRepository.ExistsAsync(command.UserId, cancellationToken))
        {
            AddError(errors, "userId", $"User '{command.UserId}' does not exist.");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(ToPayload(errors));
        }

        var existing = await _todoRepository.GetByIdAsync(command.Id, cancellationToken);

        if (existing is null)
        {
            throw new NotFoundException("Todo", command.Id);
        }

        existing.UserId = command.UserId;
        existing.Title = command.Title.Trim();
        existing.Completed = command.Completed;

        var updated = await _todoRepository.UpdateAsync(existing, cancellationToken);
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
