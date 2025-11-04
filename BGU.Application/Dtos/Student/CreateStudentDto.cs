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
    string FacultyId,
    string SpecializationId,
    string GroupId,
    string AdmissionYearId,
    EducationLanguage EducationLanguage,
    FormOfEducation FormOfEducation,
    int DecreeNumber,
    double AdmissionScore);