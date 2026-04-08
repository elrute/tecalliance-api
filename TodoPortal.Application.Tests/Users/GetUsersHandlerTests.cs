using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TodoPortal.Application.UseCases.Users.GetUsers;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using Xunit;

namespace TodoPortal.Application.Tests.Users;

public sealed class GetUsersHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly GetUsersHandler _handler;

    public GetUsersHandlerTests()
    {
        _handler = new GetUsersHandler(_userRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_When_No_Filters()
    {
        var users = new List<User> { new() { Id = 1, Name = "John", Username = "john", Email = new("john@example.com") } };
        _userRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        result.Should().ContainSingle(u => u.Id == 1 && u.Email == "john@example.com");
    }

    [Fact]
    public async Task Handle_Should_Use_Find_When_Filters()
    {
        var users = new List<User> { new() { Id = 2, Name = "Jane", Username = "jane", Email = new("jane@example.com") } };
        _userRepository.Setup(r => r.FindAsync("jane@example.com", "jane", It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _handler.Handle(new GetUsersQuery(" jane@example.com ", " jane "), CancellationToken.None);

        result.Should().ContainSingle(u => u.Username == "jane");
        _userRepository.Verify(r => r.FindAsync("jane@example.com", "jane", It.IsAny<CancellationToken>()), Times.Once);
    }
}
