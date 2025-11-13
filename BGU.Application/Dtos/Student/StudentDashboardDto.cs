using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Student;

public record StudentDashboardDto(string StudentName, IEnumerable<TodaysClassesDto>? Classes);