namespace BGU.Application.Dtos.Student;

public record StudentAcademicInfoDto(
    string Name,
    string Surname,
    string UserName,
    string StudentId,
    double Gpa,
    string EducationLevel,
    int AdmissionYear,
    string Faculty,
    string Specialization
);