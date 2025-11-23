using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record TeacherScheduleResponse(
    TeacherScheduleDto? TeacherScheduleDto,
    string Message,
    bool IsSucceeded,
    int StatusCode);