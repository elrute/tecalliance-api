namespace TodoPortal.Api.Requests.Todos;

public sealed record PatchTodoRequest(int? UserId, string? Title, bool? Completed);
