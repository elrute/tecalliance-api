using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Todos.DeleteTodo;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Todos;

public sealed class DeleteTodoHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly DeleteTodoHandler _handler;

    public DeleteTodoHandlerTests()
    {
        _handler = new DeleteTodoHandler(_todoRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_When_Found()
    {
        _todoRepository.Setup(r => r.DeleteAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await _handler.Handle(new DeleteTodoCommand(5), CancellationToken.None);

        _todoRepository.Verify(r => r.DeleteAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        _todoRepository.Setup(r => r.DeleteAsync(6, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var act = () => _handler.Handle(new DeleteTodoCommand(6), CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}
