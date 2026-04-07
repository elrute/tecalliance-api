namespace TodoPortal.Application.UseCases.Users.GetUsers;

public sealed record GetUsersQuery(string? Email = null, string? Username = null)
{
    public bool HasFilters => !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Username);
}
