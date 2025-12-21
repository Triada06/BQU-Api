namespace BGU.Application.Dtos.Dean;

public sealed record DeanProfileDto(
    string Name,
    string Surname,
    string EmployeeId,
    string Faculty,
    string Email,
    string PhoneNumber,
    string FinCode,
    DateTime BirthDate);