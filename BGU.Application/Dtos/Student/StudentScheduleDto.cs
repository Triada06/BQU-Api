using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Student;

public record StudentScheduleDto(string Today, IEnumerable<TodaysClassesDto>? Classes);