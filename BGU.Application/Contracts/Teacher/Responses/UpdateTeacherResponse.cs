using BGU.Application.Dtos.Teacher;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record UpdateTeacherResponse(
    string? TeacherId,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);