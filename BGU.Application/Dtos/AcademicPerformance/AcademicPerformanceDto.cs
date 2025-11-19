using BGU.Core.Entities;
using BGU.Core.Enums;

namespace BGU.Application.Dtos.AcademicPerformance;

public sealed record AcademicPerformanceDto(
    string ClassName,
    string GroupName,
    string ProfessorName,
    int Credits,
    int Hours,
    double OverallScore,
    IEnumerable<int> SeminarGrades,
    IEnumerable<int> Colloquium,
    int IndependentWorks,
    int Absences,
    int ClassCount);