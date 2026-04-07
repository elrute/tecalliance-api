using TodoPortal.Application.Abstractions;

namespace TodoPortal.Api.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
