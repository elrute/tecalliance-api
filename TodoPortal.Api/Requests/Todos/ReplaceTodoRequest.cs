namespace TodoPortal.Api.Requests.Todos;

public sealed record ReplaceTodoRequest(int UserId, string Title, bool Completed);
