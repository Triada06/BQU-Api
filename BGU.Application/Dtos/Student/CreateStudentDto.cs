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
    // string FacultyId,
    string FacultyName,
    // string SpecializationId,
    string SpecializationName,
    // string GroupId,
    string GroupName,
    // string AdmissionYearId,
    string AdmissionYear,
    EducationLanguage EducationLanguage,
    FormOfEducation FormOfEducation,
    int DecreeNumber,
    double AdmissionScore);