using BGU.Application.Dtos.Teacher;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record GetByIdTeacherResponse(
    GetTeacherDto? TeacherDto,
    string Message,
    bool IsSucceeded,
    StatusCode StatusCode);