namespace TodoPortal.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
