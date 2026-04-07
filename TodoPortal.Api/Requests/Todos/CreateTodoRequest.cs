namespace TodoPortal.Api.Requests.Todos;

public sealed record CreateTodoRequest(int UserId, string Title, bool Completed);
