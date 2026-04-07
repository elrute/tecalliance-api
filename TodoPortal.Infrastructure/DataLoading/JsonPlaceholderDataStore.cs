using TodoPortal.Domain.Entities;
using TodoPortal.Domain.ValueObjects;

namespace TodoPortal.Infrastructure.DataLoading;

public sealed class JsonPlaceholderDataStore
{
    private readonly object _todosLock = new();
    private readonly List<Todo> _todos;
    private readonly List<User> _users;
    private int _nextTodoId;

    public JsonPlaceholderDataStore()
        : this(JsonPlaceholderDataLoader.Load())
    {
    }

    public JsonPlaceholderDataStore(JsonPlaceholderSeedData seedData)
    {
        _users = seedData.Users.Select(CloneUser).ToList();
        _todos = seedData.Todos.Select(CloneTodo).ToList();
        _nextTodoId = _todos.Select(t => t.Id).DefaultIfEmpty(0).Max() + 1;
    }

    public IReadOnlyCollection<User> GetUsers()
        => _users.Select(CloneUser).ToArray();

    public User? GetUserById(int id)
    {
        var user = _users.FirstOrDefault(user => user.Id == id);
        return user is null ? null : CloneUser(user);
    }

    public IReadOnlyCollection<User> FindUsers(string? email, string? username)
    {
        var query = _users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(user =>
                string.Equals(user.Email.Value, email, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            query = query.Where(user =>
                string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        return query.Select(CloneUser).ToArray();
    }

    public bool UserExists(int id)
        => _users.Any(user => user.Id == id);

    public IReadOnlyCollection<Todo> GetTodos()
    {
        lock (_todosLock)
        {
            return _todos.Select(CloneTodo).ToArray();
        }
    }

    public IReadOnlyCollection<Todo> FindTodos(int? userId, bool? completed)
    {
        lock (_todosLock)
        {
            IEnumerable<Todo> query = _todos;

            if (userId.HasValue)
            {
                query = query.Where(todo => todo.UserId == userId.Value);
            }

            if (completed.HasValue)
            {
                query = query.Where(todo => todo.Completed == completed.Value);
            }

            return query.Select(CloneTodo).ToArray();
        }
    }

    public IReadOnlyCollection<Todo> GetTodosByUserId(int userId)
    {
        lock (_todosLock)
        {
            return _todos
                .Where(todo => todo.UserId == userId)
                .Select(CloneTodo)
                .ToArray();
        }
    }

    public Todo? GetTodoById(int id)
    {
        lock (_todosLock)
        {
            var todo = _todos.FirstOrDefault(todo => todo.Id == id);
            return todo is null ? null : CloneTodo(todo);
        }
    }

    public Todo AddTodo(Todo todo)
    {
        lock (_todosLock)
        {
            var entity = CloneTodo(todo);
            entity.Id = _nextTodoId++;
            _todos.Add(entity);
            return CloneTodo(entity);
        }
    }

    public Todo? UpdateTodo(Todo todo)
    {
        lock (_todosLock)
        {
            var existing = _todos.FirstOrDefault(t => t.Id == todo.Id);
            if (existing is null)
            {
                return null;
            }

            existing.UserId = todo.UserId;
            existing.Title = todo.Title;
            existing.Completed = todo.Completed;

            return CloneTodo(existing);
        }
    }

    public bool DeleteTodo(int id)
    {
        lock (_todosLock)
        {
            var index = _todos.FindIndex(t => t.Id == id);
            if (index < 0)
            {
                return false;
            }

            _todos.RemoveAt(index);
            return true;
        }
    }

    private static Todo CloneTodo(Todo source)
        => new()
        {
            Id = source.Id,
            UserId = source.UserId,
            Title = source.Title,
            Completed = source.Completed
        };

    private static User CloneUser(User source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            Username = source.Username,
            Email = new Email(source.Email.Value),
            Address = new Address
            {
                Street = source.Address.Street,
                Suite = source.Address.Suite,
                City = source.Address.City,
                Zipcode = source.Address.Zipcode,
                Geo = new Geo
                {
                    Lat = source.Address.Geo.Lat,
                    Lng = source.Address.Geo.Lng
                }
            },
            Phone = source.Phone,
            Website = source.Website,
            Company = new Company
            {
                Name = source.Company.Name,
                CatchPhrase = source.Company.CatchPhrase,
                Bs = source.Company.Bs
            }
        };
}
