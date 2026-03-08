using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Student;

public record StudentScheduleDto(string Today, bool IsUpperWeek, IEnumerable<TodaysClassesDto>? Classes);