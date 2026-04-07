using System.Text.Json;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.ValueObjects;

namespace TodoPortal.Infrastructure.DataLoading;

internal static class JsonPlaceholderDataLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static JsonPlaceholderSeedData Load(string? basePath = null)
    {
        basePath ??= Path.Combine(AppContext.BaseDirectory, "Data");

        var users = LoadUsers(Path.Combine(basePath, "users.json"));
        var todos = LoadTodos(Path.Combine(basePath, "todos.json"));

        return new JsonPlaceholderSeedData(users, todos);
    }

    private static IReadOnlyList<User> LoadUsers(string path)
    {
        using var stream = File.OpenRead(path);
        var documents = JsonSerializer.Deserialize<UserDocument[]>(stream, SerializerOptions) ?? Array.Empty<UserDocument>();
        return documents.Select(MapUser).ToArray();
    }

    private static IReadOnlyList<Todo> LoadTodos(string path)
    {
        using var stream = File.OpenRead(path);
        var documents = JsonSerializer.Deserialize<TodoDocument[]>(stream, SerializerOptions) ?? Array.Empty<TodoDocument>();
        return documents.Select(MapTodo).ToArray();
    }

    private static User MapUser(UserDocument document)
    {
        var user = new User
        {
            Id = document.Id,
            Name = document.Name ?? string.Empty,
            Username = document.Username ?? string.Empty,
            Email = new Email(document.Email ?? string.Empty),
            Phone = document.Phone ?? string.Empty,
            Website = document.Website ?? string.Empty,
            Address = new Address
            {
                Street = document.Address?.Street ?? string.Empty,
                Suite = document.Address?.Suite ?? string.Empty,
                City = document.Address?.City ?? string.Empty,
                Zipcode = document.Address?.Zipcode ?? string.Empty,
                Geo = new Geo
                {
                    Lat = document.Address?.Geo?.Lat ?? string.Empty,
                    Lng = document.Address?.Geo?.Lng ?? string.Empty
                }
            },
            Company = new Company
            {
                Name = document.Company?.Name ?? string.Empty,
                CatchPhrase = document.Company?.CatchPhrase ?? string.Empty,
                Bs = document.Company?.Bs ?? string.Empty
            }
        };

        return user;
    }

    private static Todo MapTodo(TodoDocument document)
        => new()
        {
            Id = document.Id,
            UserId = document.UserId,
            Title = document.Title ?? string.Empty,
            Completed = document.Completed
        };

    private sealed record UserDocument(
        int Id,
        string Name,
        string Username,
        string Email,
        AddressDocument Address,
        string Phone,
        string Website,
        CompanyDocument Company);

    private sealed record AddressDocument(
        string Street,
        string Suite,
        string City,
        string Zipcode,
        GeoDocument Geo);

    private sealed record GeoDocument(string Lat, string Lng);

    private sealed record CompanyDocument(string Name, string CatchPhrase, string Bs);

    private sealed record TodoDocument(int UserId, int Id, string Title, bool Completed);
}
