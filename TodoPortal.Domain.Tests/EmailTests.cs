using FluentAssertions;
using TodoPortal.Domain.Exceptions;
using TodoPortal.Domain.ValueObjects;
using Xunit;

namespace TodoPortal.Domain.Tests.ValueObjects;

public sealed class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    public void Constructor_Should_Set_Value_When_Email_Is_Valid(string input)
    {
        var email = new Email(input);
        email.Value.Should().Be(input.Trim());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void Constructor_Should_Throw_When_Email_Is_Invalid(string input)
    {
        Action act = () => new Email(input);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equals_Should_Be_Case_Insensitive()
    {
        var first = new Email("user@example.com");
        var second = new Email("USER@example.com");
        first.Should().Be(second);
    }
}
