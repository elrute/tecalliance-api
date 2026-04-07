using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Entities;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Users.GetUsers;

public sealed class GetUsersHandler
{
    private readonly IUserRepository _userRepository;

    public GetUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<UserDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<User> users;

        if (query.HasFilters)
        {
            var email = query.Email?.Trim();
            var username = query.Username?.Trim();
            users = await _userRepository.FindAsync(email, username, cancellationToken);
        }
        else
        {
            users = await _userRepository.GetAllAsync(cancellationToken);
        }

        return users.ToUserDtos();
    }
}
