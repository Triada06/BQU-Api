namespace BGU.Application.Dtos.Dean;

public sealed record DeanProfileDto(
    string Name,
    string Surname,
    string EmployeeId,
    string StateName,
    string Faculty,
    string Specialization,
    string Email,
    DateTime BirthDate);