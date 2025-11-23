using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Teacher;

public sealed record TeacherScheduleDto(string Today, IEnumerable<TodaysClassesDto>? Classes);