using BGU.Application.Dtos.IndependentWorks;

namespace BGU.Application.Dtos.AcademicPerformance;

// public sealed record AcademicPerformanceDto(
//     string ClassName,
//     string GroupName,
//     string ProfessorName,
//     int Credits,
//     int Hours,
//     double OverallScore,
//     IEnumerable<int> SeminarGrades,
//     IEnumerable<int> Colloquium,
//     int IndependentWorks,
//     int Absences,
//     int ClassCount);
//     
public sealed record AcademicPerformanceDto(
    string ClassName,
    string GroupName,
    string ProfessorName,
    int Credits,
    int Hours,
    double OverallScore,
    IEnumerable<int> SeminarGrades,
    IEnumerable<int> Colloquium,
    IEnumerable<GetIndependentWorkDto> IndependentWorks,
    IEnumerable<bool> Attendances,
    int ClassCount);