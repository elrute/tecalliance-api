using FluentAssertions;
using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Todos.CreateTodo;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.UseCases.Todos;

public sealed class CreateTodoHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly CreateTodoHandler _handler;

    public CreateTodoHandlerTests()
    {
        _handler = new CreateTodoHandler(_todoRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Todo_When_Valid()
    {
        var command = new CreateTodoCommand(1, "New task", false);
        var todo = new Todo { Id = 101, UserId = 1, Title = "New task", Completed = false };

        _userRepository.Setup(repo => repo.ExistsAsync(command.UserId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);
        _todoRepository.Setup(repo => repo.AddAsync(It.IsAny<Todo>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(todo);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(101);
        result.Title.Should().Be("New task");
        _todoRepository.Verify(repo => repo.AddAsync(It.Is<Todo>(t => t.UserId == 1 && t.Title == "New task"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Does_Not_Exist()
    {
        var command = new CreateTodoCommand(99, "New task", false);
        _userRepository.Setup(repo => repo.ExistsAsync(command.UserId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        _todoRepository.Verify(repo => repo.AddAsync(It.IsAny<Todo>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
