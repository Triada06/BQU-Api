using BGU.Application.Dtos.AcademicPerformance;

namespace BGU.Application.Dtos.Student;

public sealed record StudentGradesDto(IEnumerable<AcademicPerformanceDto> AcademicPerformance);