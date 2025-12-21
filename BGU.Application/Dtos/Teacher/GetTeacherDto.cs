using BGU.Core.Enums;

namespace BGU.Application.Dtos.Teacher;

public sealed record GetTeacherDto(
    string Id,
    string Email,
    string Name,
    string Surname,
    string MiddleName,
    string PinCode,
    char Gender,
    DateTime BornDate,
    string DepartmentId,
    TeachingPosition Position);