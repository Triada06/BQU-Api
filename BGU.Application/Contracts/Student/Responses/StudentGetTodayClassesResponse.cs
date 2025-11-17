using BGU.Application.Dtos.Class;

namespace BGU.Application.Contracts.Student.Responses;

public record StudentGetTodayClassesResponse(
    IEnumerable<ClassDto>? Dto,
    string Message,
    bool IsSucceeded,
    int StatusCode);