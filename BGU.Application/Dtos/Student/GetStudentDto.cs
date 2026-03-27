namespace BGU.Application.Dtos.Student;

public sealed record GetStudentDto(
    string Id,
    string FullName,
    string UserName,
    string GroupName,
    int Year,
    string Speciality,
    string YearOfAdmission,
    double AdmissionScore);