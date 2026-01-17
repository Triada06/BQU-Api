using BGU.Core.Enums;

namespace BGU.Application.Dtos.Teacher;

public sealed record GetTeacherDto(
    string Id,
    string Name,
    string Surname,
    string MiddleName,
    string UserName,
    char Gender,
    string DepartmentId,
    TeachingPosition Position);