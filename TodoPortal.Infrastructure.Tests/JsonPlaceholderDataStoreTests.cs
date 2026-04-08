using FluentAssertions;
using TodoPortal.Domain.Entities;
using TodoPortal.Infrastructure.DataLoading;
using Xunit;

namespace TodoPortal.Infrastructure.Tests.DataLoading;

public sealed class JsonPlaceholderDataStoreTests
{
    private readonly JsonPlaceholderDataStore _store;

    public JsonPlaceholderDataStoreTests()
    {
        _store = new JsonPlaceholderDataStore();
    }

    [Fact]
    public void Seed_Data_Should_Be_Loaded()
    {
        var users = _store.GetUsers();
        var todos = _store.GetTodos();

        users.Should().HaveCountGreaterOrEqualTo(10);
        todos.Should().HaveCountGreaterOrEqualTo(200);
    }

    [Fact]
    public void AddTodo_Should_Assign_New_Id()
    {
        var todo = new Todo { UserId = 1, Title = "test", Completed = false };
        var created = _store.AddTodo(todo);

        created.Id.Should().BeGreaterThan(0);
        _store.GetTodoById(created.Id).Should().NotBeNull();
    }

    [Fact]
    public void DeleteTodo_Should_Remove_Item()
    {
        var todo = new Todo { UserId = 1, Title = "temporary", Completed = false };
        var created = _store.AddTodo(todo);

        _store.DeleteTodo(created.Id).Should().BeTrue();
        _store.GetTodoById(created.Id).Should().BeNull();
    }
}
