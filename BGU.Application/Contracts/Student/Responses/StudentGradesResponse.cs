using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.Student;

namespace BGU.Application.Contracts.Student.Responses;

public sealed record StudentGradesResponse(
    StudentGradesDto? Items,
    IEnumerable<ClassSessions>? Sessions,
    string Message,
    bool IsSucceeded,
    int StatusCode);