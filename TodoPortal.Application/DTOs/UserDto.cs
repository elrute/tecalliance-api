namespace TodoPortal.Application.DTOs;

public sealed record UserDto(
    int Id,
    string Name,
    string Username,
    string Email,
    AddressDto Address,
    string Phone,
    string Website,
    CompanyDto Company);
