namespace BGU.Application.Dtos.Student;

public sealed record GetStudentDto(
    string FullName,
    string UserName,
    string GroupName,
    int Year,
    string Speciality,
    string YearOfAdmission,
    double AdmissionScore);