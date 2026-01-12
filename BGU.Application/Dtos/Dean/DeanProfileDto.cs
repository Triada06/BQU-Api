namespace BGU.Application.Dtos.Dean;

public sealed record DeanProfileDto(
    string Name,
    string Surname,
    string FinCode,
    string Faculty,
    string RoleName
);