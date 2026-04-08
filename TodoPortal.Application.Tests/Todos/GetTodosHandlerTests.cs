using FluentAssertions;
using Moq;
using TodoPortal.Application.UseCases.Todos.GetTodos;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Todos;

public sealed class GetTodosHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly GetTodosHandler _handler;

    public GetTodosHandlerTests()
    {
        _handler = new GetTodosHandler(_todoRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_When_No_Filters()
    {
        var todos = new List<Todo> { new() { Id = 1, Title = "a", UserId = 1, Completed = false } };
        _todoRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(todos);

        var result = await _handler.Handle(new GetTodosQuery(), CancellationToken.None);

        result.Should().ContainSingle(dto => dto.Id == 1 && dto.Title == "a");
        _todoRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _todoRepository.Verify(r => r.FindAsync(It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Use_Find_When_Filters()
    {
        var todos = new List<Todo> { new() { Id = 2, Title = "filtered", UserId = 5, Completed = true } };
        _todoRepository.Setup(r => r.FindAsync(5, true, It.IsAny<CancellationToken>())).ReturnsAsync(todos);

        var result = await _handler.Handle(new GetTodosQuery(5, true), CancellationToken.None);

        result.Should().ContainSingle(dto => dto.Id == 2 && dto.Completed);
        _todoRepository.Verify(r => r.FindAsync(5, true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
