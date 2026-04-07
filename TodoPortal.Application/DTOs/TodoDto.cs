namespace TodoPortal.Application.DTOs;

public sealed record TodoDto(
    int UserId,
    int Id,
    string Title,
    bool Completed);
