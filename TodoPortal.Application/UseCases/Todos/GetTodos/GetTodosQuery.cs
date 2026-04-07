namespace TodoPortal.Application.UseCases.Todos.GetTodos;

public sealed record GetTodosQuery(int? UserId = null, bool? Completed = null)
{
    public bool HasFilters => UserId.HasValue || Completed.HasValue;
}
