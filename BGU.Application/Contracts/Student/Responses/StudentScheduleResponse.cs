using BGU.Application.Dtos.Student;

namespace BGU.Application.Contracts.Student.Responses;

public record StudentScheduleResponse(
    StudentScheduleDto? StudentSchedule,
    string Message,
    bool IsSucceeded,
    int StatusCode);