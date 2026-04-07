using TodoPortal.Domain.Entities;

namespace TodoPortal.Infrastructure.DataLoading;

public sealed record JsonPlaceholderSeedData(
    IReadOnlyList<User> Users,
    IReadOnlyList<Todo> Todos);
