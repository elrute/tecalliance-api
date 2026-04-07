using TodoPortal.Domain.Entities;

namespace TodoPortal.Domain.Ports;

public interface ITodoRepository
{
    Task<IReadOnlyCollection<Todo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Todo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Todo>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Todo>> FindAsync(int? userId, bool? completed, CancellationToken cancellationToken = default);
    Task<Todo> AddAsync(Todo todo, CancellationToken cancellationToken = default);
    Task<Todo> UpdateAsync(Todo todo, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}