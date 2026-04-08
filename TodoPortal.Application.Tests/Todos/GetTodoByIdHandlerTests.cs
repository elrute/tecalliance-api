using FluentAssertions;
using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Todos.GetTodoById;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Todos;

public sealed class GetTodoByIdHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly GetTodoByIdHandler _handler;

    public GetTodoByIdHandlerTests()
    {
        _handler = new GetTodoByIdHandler(_todoRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Todo_When_Found()
    {
        var todo = new Todo { Id = 42, Title = "demo", UserId = 1, Completed = true };
        _todoRepository.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>())).ReturnsAsync(todo);

        var result = await _handler.Handle(new GetTodoByIdQuery(42), CancellationToken.None);

        result.Id.Should().Be(42);
        result.Completed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        _todoRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Todo?)null);

        Func<Task> act = () => _handler.Handle(new GetTodoByIdQuery(99), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
