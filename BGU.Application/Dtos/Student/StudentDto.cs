using BGU.Core.Enums;

namespace BGU.Application.Dtos.Student;

public record StudentDto(
    string Name,
    string Surname,
    string MiddleName,
    string UserName,
    char Gender,
    string GroupName,
    int DecreeNumber,
    double AdmissionScore,
    FormOfEducation FormOfEducation
);