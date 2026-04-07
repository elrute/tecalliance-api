namespace TodoPortal.Application.UseCases.Todos.PatchTodo;

public sealed record PatchTodoCommand(
    int Id,
    int? UserId,
    string? Title,
    bool? Completed);
