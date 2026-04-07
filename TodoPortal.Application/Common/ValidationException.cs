using System.Collections.ObjectModel;

namespace TodoPortal.Application.Common;

public sealed class ValidationException : Exception
{
    public ValidationException(IDictionary<string, string[]> errors)
        : this("Validation failed.", errors)
    {
    }

    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = new ReadOnlyDictionary<string, string[]>(
            new Dictionary<string, string[]>(errors, StringComparer.OrdinalIgnoreCase));
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public static ValidationException Single(string propertyName, string errorMessage)
    {
        var payload = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [propertyName] = new[] { errorMessage }
        };

        return new ValidationException(payload);
    }
}
