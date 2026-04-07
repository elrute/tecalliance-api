namespace TodoPortal.Application.Common;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object key)
        : base($"Resource '{resourceName}' with key '{key}' was not found.")
    {
        ResourceName = resourceName;
        Key = key;
    }

    public string ResourceName { get; }

    public object Key { get; }
}
