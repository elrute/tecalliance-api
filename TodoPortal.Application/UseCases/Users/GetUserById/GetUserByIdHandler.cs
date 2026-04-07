using TodoPortal.Application.Common;
using TodoPortal.Application.DTOs;
using TodoPortal.Application.Mappings;
using TodoPortal.Domain.Ports;

namespace TodoPortal.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.Id, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", query.Id);
        }

        return user.ToDto();
    }
}
