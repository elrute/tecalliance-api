using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Users.GetUserTodos;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Users;

public sealed class GetUserTodosHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly GetUserTodosHandler _handler;

    public GetUserTodosHandlerTests()
    {
        _handler = new GetUserTodosHandler(_todoRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_User_Todos()
    {
        _userRepository.Setup(r => r.ExistsAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var todos = new List<Todo> { new() { Id = 1, UserId = 3, Title = "a", Completed = false } };
        _todoRepository.Setup(r => r.GetByUserIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(todos);

        var result = await _handler.Handle(new GetUserTodosQuery(3), CancellationToken.None);

        result.Should().ContainSingle(t => t.UserId == 3);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_User_Not_Found()
    {
        _userRepository.Setup(r => r.ExistsAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var act = () => _handler.Handle(new GetUserTodosQuery(4), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _todoRepository.Verify(r => r.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
