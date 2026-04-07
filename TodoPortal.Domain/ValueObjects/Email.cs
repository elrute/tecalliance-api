using System.Net.Mail;
using TodoPortal.Domain.Exceptions;

namespace TodoPortal.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email is required.");

        var trimmed = value.Trim();

        if (!IsValid(trimmed))
            throw new DomainException("Email is not valid.");

        Value = trimmed;
    }

    public static bool IsValid(string value)
    {
        try
        {
            var address = new MailAddress(value);
            return address.Address.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public static implicit operator string(Email email) => email.Value;
}
