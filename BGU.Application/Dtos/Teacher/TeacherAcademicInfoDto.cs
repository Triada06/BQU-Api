namespace BGU.Application.Dtos.Teacher;

public sealed record TeacherAcademicInfoDto(
    string Name,
    string Surname,
    string EmployeeId,
    string StateName,
    string Faculty,
    string Specialization);