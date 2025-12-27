using BGU.Core.Enums;

namespace BGU.Application.Dtos.Student;

public record CreateStudentDto(
    string Name,
    string Surname,
    string MiddleName,
    string Email,
    string PinCode,
    char Gender,
    DateTime BornDate,
    string GroupName,
    int DecreeNumber,
    double AdmissionScore,
    FormOfEducation FormOfEducation
    );