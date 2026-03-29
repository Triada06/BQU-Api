using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Student;

public sealed record GetStudentPageDto(
    string GroupCode,
    string SpecializationName,
    string AdmissionYear,
    int Course,
    double AdmissionScore,
    string? Email,
    IEnumerable<TodaysClassesDto> TodayClasses,
    StudentGradesDto? Grades);