using TodoPortal.Application.Common;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Todos.CreateTodo;

public sealed class CreateTodoHandler
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;

    public CreateTodoHandler(ITodoRepository todoRepository, IUserRepository userRepository)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
    }

    public async Task<TodoDto> Handle(CreateTodoCommand command, CancellationToken cancellationToken = default)
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

        var todo = new Todo
        {
            UserId = command.UserId,
            Title = command.Title.Trim(),
            Completed = command.Completed
        };

        var created = await _todoRepository.AddAsync(todo, cancellationToken);
        return created.ToDto();
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
