namespace TodoPortal.Application.UseCases.Todos.CreateTodo;

public sealed record CreateTodoCommand(int UserId, string Title, bool Completed);
