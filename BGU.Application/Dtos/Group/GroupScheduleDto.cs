using BGU.Application.Dtos.Class;

namespace BGU.Application.Dtos.Group;

public record GroupScheduleDto(string Today, bool IsUpperWeek, IEnumerable<TodaysClassesDto>? Classes);

public record GroupScheduleResponse(
    GroupScheduleDto? GroupSchedule,
    string Message,
    bool IsSucceeded,
    int StatusCode);