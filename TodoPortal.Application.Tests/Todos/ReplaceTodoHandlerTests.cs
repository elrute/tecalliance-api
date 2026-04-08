using FluentAssertions;
using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Todos.ReplaceTodo;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Todos;

public sealed class ReplaceTodoHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly ReplaceTodoHandler _handler;

    public ReplaceTodoHandlerTests()
    {
        _handler = new ReplaceTodoHandler(_todoRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Todo_When_Valid()
    {
        var command = new ReplaceTodoCommand(5, 2, " updated ", true);
        var existing = new Todo { Id = 5, UserId = 1, Title = "old", Completed = false };
        var updated = new Todo { Id = 5, UserId = 2, Title = "updated", Completed = true };

        _userRepository.Setup(r => r.ExistsAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _todoRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _todoRepository.Setup(r => r.UpdateAsync(It.IsAny<Todo>(), It.IsAny<CancellationToken>())).ReturnsAsync(updated);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.UserId.Should().Be(2);
        result.Title.Should().Be("updated");
        _todoRepository.Verify(r => r.UpdateAsync(It.Is<Todo>(t => t.Title == "updated" && t.UserId == 2), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Validation_When_User_Missing()
    {
        var command = new ReplaceTodoCommand(5, 999, "demo", false);
        _userRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFound_When_Todo_Missing()
    {
        var command = new ReplaceTodoCommand(5, 1, "demo", false);
        _userRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _todoRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((Todo?)null);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
