namespace BGU.Application.Dtos.Student;

public record StudentAcademicInfoDto(
    string Fin,
    string Name,
    string Surname,
    string StudentId,
    double Gpa,
    string EducationLevel,
    int AdmissionYear,
    string Faculty,
    string Specialization);