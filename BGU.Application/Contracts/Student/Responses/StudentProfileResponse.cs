using BGU.Application.Dtos.Student;

namespace BGU.Application.Contracts.Student.Responses;

public sealed record StudentProfileResponse(
    StudentAcademicInfoDto? StudentAcademicInfoDto,
    string Message,
    bool IsSucceeded,
    int StatusCode);