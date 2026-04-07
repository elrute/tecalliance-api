using TodoPortal.Application.DTOs;
using TodoPortal.Domain.Entities;

namespace TodoPortal.Application.Mappings;

public static class DomainToDtoMapper
{
    public static TodoDto ToDto(this Todo todo)
    {
        ArgumentNullException.ThrowIfNull(todo);
        return new TodoDto(todo.UserId, todo.Id, todo.Title, todo.Completed);
    }

    public static IReadOnlyCollection<TodoDto> ToTodoDtos(this IEnumerable<Todo> todos)
        => todos.Select(ToDto).ToArray();

    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto(
            user.Id,
            user.Name,
            user.Username,
            user.Email?.Value ?? string.Empty,
            user.Address.ToDto(),
            user.Phone,
            user.Website,
            user.Company.ToDto());
    }

    public static IReadOnlyCollection<UserDto> ToUserDtos(this IEnumerable<User> users)
        => users.Select(ToDto).ToArray();

    private static AddressDto ToDto(this Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        return new AddressDto(
            address.Street,
            address.Suite,
            address.City,
            address.Zipcode,
            address.Geo.ToDto());
    }

    private static CompanyDto ToDto(this Company company)
    {
        ArgumentNullException.ThrowIfNull(company);

        return new CompanyDto(
            company.Name,
            company.CatchPhrase,
            company.Bs);
    }

    private static GeoDto ToDto(this Geo geo)
    {
        ArgumentNullException.ThrowIfNull(geo);
        return new GeoDto(geo.Lat, geo.Lng);
    }
}
