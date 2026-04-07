using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using TodoPortal.Infrastructure.DataLoading;

namespace TodoPortal.Infrastructure.Repositories;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly JsonPlaceholderDataStore _store;

    public InMemoryTodoRepository(JsonPlaceholderDataStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyCollection<Todo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var todos = _store.GetTodos();
        return Task.FromResult(todos);
    }

    public Task<Todo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var todo = _store.GetTodoById(id);
        return Task.FromResult(todo);
    }

    public Task<IReadOnlyCollection<Todo>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var todos = _store.GetTodosByUserId(userId);
        return Task.FromResult(todos);
    }

    public Task<IReadOnlyCollection<Todo>> FindAsync(int? userId, bool? completed, CancellationToken cancellationToken = default)
    {
        var todos = _store.FindTodos(userId, completed);
        return Task.FromResult(todos);
    }

    public Task<Todo> AddAsync(Todo todo, CancellationToken cancellationToken = default)
    {
        var created = _store.AddTodo(todo);
        return Task.FromResult(created);
    }

    public Task<Todo> UpdateAsync(Todo todo, CancellationToken cancellationToken = default)
    {
        var updated = _store.UpdateTodo(todo) ?? throw new InvalidOperationException($"Todo '{todo.Id}' does not exist.");
        return Task.FromResult(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var removed = _store.DeleteTodo(id);
        return Task.FromResult(removed);
    }
}
