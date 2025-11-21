using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Contracts.Teacher.Responses;

public sealed record TeacherProfileResponse(
    TeacherPersonalInfoDto? TeacherPersonalInfo,
    TeacherAcademicInfoDto? TeacherAcademicInfo,
    string Message,
    bool IsSucceeded,
    int StatusCode);