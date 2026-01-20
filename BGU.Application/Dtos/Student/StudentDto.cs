namespace BGU.Application.Dtos.Student;

public record StudentDto(
    string Name,
    string Surname,
    string MiddleName,
    string UserName,
    string GroupName,
    double AdmissionScore
);