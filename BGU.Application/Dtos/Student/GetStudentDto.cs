namespace BGU.Application.Dtos.Student;

public sealed record GetStudentDto(
    string StudentId,
    string FullName,
    string GroupName,
    int Year,
    char Gender,
    string Speciality,
    string YearOfAdmission,
    double AdmissionScore);