using FluentAssertions;
using Moq;
using TodoPortal.Application.Common;
using TodoPortal.Application.UseCases.Users.GetUserById;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;
using TodoPortal.Domain.ValueObjects;
using Xunit;

namespace TodoPortal.Application.Tests.Users;

public sealed class GetUserByIdHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _handler = new GetUserByIdHandler(_userRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_User_When_Found()
    {
        var user = new User { Id = 10, Name = "Test", Username = "test", Email = new Email("test@example.com") };
        _userRepository.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var dto = await _handler.Handle(new GetUserByIdQuery(10), CancellationToken.None);

        dto.Id.Should().Be(10);
        dto.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Not_Found()
    {
        _userRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var act = () => _handler.Handle(new GetUserByIdQuery(99), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
