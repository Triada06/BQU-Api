using BGU.Application.Dtos.Teacher;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record GetAllTeachersResponse(
    IEnumerable<GetTeacherDto> Teachers,
    string Message,
    bool IsSucceeded,
    StatusCode StatusCode);