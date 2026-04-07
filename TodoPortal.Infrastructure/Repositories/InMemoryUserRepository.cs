using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using TodoPortal.Infrastructure.DataLoading;

namespace TodoPortal.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly JsonPlaceholderDataStore _store;

    public InMemoryUserRepository(JsonPlaceholderDataStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = _store.GetUsers();
        return Task.FromResult(users);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = _store.GetUserById(id);
        return Task.FromResult(user);
    }

    public Task<IReadOnlyCollection<User>> FindAsync(string? email, string? username, CancellationToken cancellationToken = default)
    {
        var users = _store.FindUsers(email?.Trim(), username?.Trim());
        return Task.FromResult(users);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var exists = _store.UserExists(id);
        return Task.FromResult(exists);
    }
}
