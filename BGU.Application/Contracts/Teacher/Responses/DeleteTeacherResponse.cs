using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record DeleteTeacherResponse(
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);