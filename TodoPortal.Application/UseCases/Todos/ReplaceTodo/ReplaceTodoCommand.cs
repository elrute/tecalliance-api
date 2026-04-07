namespace TodoPortal.Application.UseCases.Todos.ReplaceTodo;

public sealed record ReplaceTodoCommand(int Id, int UserId, string Title, bool Completed);
