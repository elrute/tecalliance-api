using FluentAssertions;
using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Todos.PatchTodo;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Todos;

public sealed class PatchTodoHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly PatchTodoHandler _handler;

    public PatchTodoHandlerTests()
    {
        _handler = new PatchTodoHandler(_todoRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Selected_Fields()
    {
        var command = new PatchTodoCommand(5, 3, " new title ", null);
        var existing = new Todo { Id = 5, UserId = 1, Title = "old", Completed = false };
        var updated = new Todo { Id = 5, UserId = 3, Title = "new title", Completed = false };

        _userRepository.Setup(r => r.ExistsAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _todoRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _todoRepository.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).ReturnsAsync(updated);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.UserId.Should().Be(3);
        result.Title.Should().Be("new title");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_No_Fields()
    {
        var command = new PatchTodoCommand(1, null, null, null);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Invalid()
    {
        var command = new PatchTodoCommand(1, 999, null, null);
        _userRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
