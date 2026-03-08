using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Teacher;

public sealed record TeacherScheduleDto(string Today, bool IsUpperWeek, IEnumerable<TodaysClassesDto>? Classes);